using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Sepia.Calendaring.Serialization
{
    /// <summary>
    ///   A content line consists of a <see cref="Name"/>, optional <see cref="Parameters"/> and one or more <see cref="Values"/>.
    /// </summary>
    /// <remarks>
    ///   The iCalendar object is organized into individual lines of text,
    ///   called content lines.  Content lines are delimited by a line break,
    ///   which is a CRLF sequence (CR character followed by LF character).
    /// </remarks>
    public class ContentLine
    {
        static readonly Regex WeekPeriod = new Regex(@"(?<weeks>\d+)W((?<days>\d+)D)?");

        volatile NameValueCollection parameters;

        /// <summary>
        ///   Creates a new instance of the <see cref="ContentLine"/> class.
        /// </summary>
        public ContentLine()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="ContentLine"/> class from the specified <see cref="string"/>.
        /// </summary>
        /// <param name="content">
        ///   A content line string, formatted in accordance with RFC 5545.
        /// </param>
        /// <remarks>
        ///   Uses the <see cref="IcsReader"/> to parse the <paramref name="content"/>.
        /// </remarks>
        public ContentLine(string content)
        {
            Guard.IsNotNullOrWhiteSpace(content, "content");

            var that = Parse(content);
            this.Name = that.Name;
            this.Values = that.Values;
            if (that.HasParameters)
                this.parameters = that.Parameters;
        }

        /// <summary>
        ///   Converts the string representation of a iCalendar content line to its <see cref="ContentLine"/> equivalent.
        /// </summary>
        /// <param name="content">
        ///   A content line string, formatted in accordance with RFC 5545.
        /// </param>
        /// <returns>
        ///   A <see cref="ContentLine"/> that represents the <paramref name="content"/>.
        /// </returns>
        public static ContentLine Parse(string content)
        {
            using (var reader = IcsReader.Create(new StringReader(content)))
            {
                return reader.ReadContentLine();
            }
        }

        /// <summary>
        ///   The name of the content line.
        /// </summary>
        /// <remarks>
        ///   The name is either a "BEGIN" or "END" for a <see cref="Component"/> or a <see cref="PropertyName"/> or <b>null</b>.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        ///   Concatenation of the elements in <see cref="Values"/>, separated by a comma.
        /// </summary>
        public string Value 
        {
            get
            {
                if (Values == null || Values.Length == 0)
                    return string.Empty;
                if (Values.Length == 1)
                    return Values[0];

                return string.Join(",", Values);
            }
            set
            { 
                using (var reader = IcsReader.Create(new StringReader(value)))
                    Values = reader.ReadValues(); 
            } 
        }

        /// <summary>
        ///   The values for the content line.
        /// </summary>
        public string[] Values { get; set; }

        /// <summary>
        ///   The parameters of a content line.
        /// </summary>
        public NameValueCollection Parameters
        {
            get
            {
                if (parameters == null)
                {
                    lock (this)
                    {
                        if (parameters == null)
                            parameters = new NameValueCollection();
                    }
                }
                return parameters;
            }
        }

        /// <summary>
        ///   Determines if any <see cref="Parameters"/> have been specified.
        /// </summary>
        /// <value>
        ///   <b>true</b> if <see cref="Parameters"/> is not empty; otherwise, <b>false</b>.
        /// </value>
        /// <remarks>
        ///   The <see cref="NameValueCollection"/> is lazily created for <see cref="Parameters"/>.  Its recommended to use this method
        ///   instead of <c>Parameters.Count > 0</c>.
        /// </remarks>
        public bool HasParameters
        {
            get { return parameters != null; }
        }

        /// <summary>
        ///   Converts the <see cref="ContentLine"/> to a RFC 5545 line.
        /// </summary>
        /// <returns>
        ///   The iCalendar representation.
        /// </returns>
        /// <remarks>
        ///   Line folds are not applied.
        /// </remarks>
        public override string ToString()
        {
            var settings = new IcsWriterSettings() { OctetsPerLine = Int32.MaxValue };
            var s = new StringWriter();
            using (var writer = IcsWriter.Create(s, settings))
            {
                writer.Write(this);
            }
            return s.ToString().TrimEnd('\r', '\n');
        }

        /// <summary>
        ///   Converts the content to a <see cref="ITag"/>.
        /// </summary>
        /// <returns></returns>
        public T ToTag<T>() where T : ITag, new()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");

            return new T
            {
                Authority = "ietf:rfc5545",
                Name = v
            };
        }

        /// <summary>
        ///   Converts the content to an <see cref="ITag"/> enumerable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ToTags<T>() where T : ITag, new()
        {
            return Values.Select(v => new T { Authority = "ietf:rfc5545", Name = v });
        }

        /// <summary>
        ///   Converts the content to a <see cref="Date"/>.
        /// </summary>
        public Date ToDate()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");

            return Date.Parse(v, Parameters[ParameterName.TimeZoneId]);
        }

        /// <summary>
        ///   Converts the content to a local <see cref="DateTime"/>.
        /// </summary>
        public DateTime ToLocalDateTime()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");

            var d = Date.Parse(v, Parameters[ParameterName.TimeZoneId]);
            if (d.IsDateOnly || d.Value.Kind != DateTimeKind.Local)
                throw new CalendarException(string.Format("Expected a local date time not '{0}'.", d));
            return d.Value;
        }

        /// <summary>
        ///   Converts the content to a time zone offset.
        /// </summary>
        public TimeSpan ToTimeZoneOffset()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");

            bool isNegative = false;
            if (v[0] == '-')
            {
                v = v.Substring(1);
                isNegative = true;
            }
            else if (v[0] == '+')
            {
                v = v.Substring(1);
            }

            var offset = TimeSpan.ParseExact(v, new[] {"%h", "hh", "hhmm"}, CultureInfo.InvariantCulture);
            if (isNegative)
                return offset.Negate();
            return offset;
        }

        /// <summary>
        ///   Converts the content to a <see cref="Text"/>.
        /// </summary>
        public Text ToText()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");

            return new Text(Parameters[ParameterName.Language], v);
        }

        /// <summary>
        ///   Converts the content to a <see cref="Text"/> enumerable.
        /// </summary>
        public IEnumerable<Text> ToTextEnumerable()
        {
            return Values.Select(v => new Text(Parameters[ParameterName.Language], v));
        }

        /// <summary>
        ///   Converts the content a <see cref="TimeRange"/> enumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TimeRange> ToTimeRangeEnumerable()
        {
            return Values.Select(TimeRange.ParseIso8061);    
        }

        /// <summary>
        ///   Converts the content to a <see cref="GeoCoordinate"/>.
        /// </summary>
        public GeoCoordinate ToGeoCoordinate()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");
            var values = v.Split(';');
            if (values.Length != 2)
                throw new CalendarException("Expected latitude and longitude separated by a semi-colon.");
            return new GeoCoordinate(
                Double.Parse(values[0], CultureInfo.InvariantCulture),
                Double.Parse(values[1], CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///   Converts the content to a <see cref="MailAddress"/>.
        /// </summary>
        public MailAddress ToMailAddress()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");
            if (!v.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
                throw new CalendarException(string.Format("Expected a mailto URI not '{0}'.", v));

            return new MailAddress(v.Substring(7), Parameters[ParameterName.CommonName]);
        }

        /// <summary>
        ///   Converts the content to a <see cref="int"/>.
        /// </summary>
        public int ToInt32()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");

            return int.Parse(v, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///   Converts the content to a <see cref="TimeSpan"/>.
        /// </summary>
        public TimeSpan ToTimeSpan()
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");

            // XmlConvert does not cope with a leading '-'.
            if (v.StartsWith("+"))
                v = v.Substring(1);

            // XmlConvert does not cope with weeks 'P1W', but we can convert it to days 'P7D'.
            if (v.Contains("W"))
            {
                v = WeekPeriod.Replace(v, (match) =>
                {
                    var days =  Int32.Parse(match.Groups["weeks"].Value) * 7;
                    if (match.Groups["days"].Success)
                        days += Int32.Parse(match.Groups["days"].Value);
                    return days.ToString(CultureInfo.InvariantCulture) + "D";
                });
            }

            return XmlConvert.ToTimeSpan(v);
        }

        /// <summary>
        ///   Convert the content to a <see cref="bool"/>.
        /// </summary>
        /// <returns></returns>
        public bool ToBoolean()
        {
            return ToBoolean("TRUE", "FALSE");
        }

        /// <summary>
        ///   Convert the the content to a <see cref="bool"/> based on the supplied
        ///   names for true and false.
        /// </summary>
        /// <param name="trueName">The case insensitive name for <b>true</b>.</param>
        /// <param name="falseName">The case insensitive name for <b>false</b>.</param>
        public bool ToBoolean(string trueName, string falseName)
        {
            var v = Value;
            if (string.IsNullOrWhiteSpace(v))
                throw new CalendarException("Expected a non-empty value.");
            if (v.Equals(trueName, StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (v.Equals(falseName, StringComparison.InvariantCultureIgnoreCase))
                return false;

            throw new CalendarException(string.Format("Expected '{0}' or '{1}' not '{2}'.", trueName, falseName, v));
        }

        /// <summary>
        ///   Convert the content to a <see cref="Date"/> enumerable.
        /// </summary>
        public IEnumerable<Date> ToRecurrenceDates()
        {
            return Values.Select(v => Date.Parse(v, Parameters[ParameterName.TimeZoneId]));
        }
    }
}
