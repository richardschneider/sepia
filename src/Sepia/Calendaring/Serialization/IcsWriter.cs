using System;
using System.Device.Location;
using System.Globalization;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring.Serialization
{
    using System.Xml;

    /// <summary>
    ///   Writes iCalendar data to a <see cref="TextWriter"/>.
    /// </summary>
    public class IcsWriter : IDisposable
    {
        Stack<string> startedComponentNames = new Stack<string>();
        Encoding utf8 = Encoding.UTF8;
        TextWriter writer;
        IcsWriterSettings settings;
        int lineOctets;

        /// <summary>
        ///   Creates a new instance of the <see cref="IcsWriter"/> with the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">
        ///   The <see cref="TextWriter"/> to write iCalendar data.
        /// </param>
        public static IcsWriter Create(TextWriter writer)
        {
            return new IcsWriter(writer, new IcsWriterSettings());
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="IcsWriter"/> with the specified <see cref="TextWriter"/>
        ///   and <see cref="IcsWriterSettings"/>
        /// </summary>
        /// <param name="writer">
        ///   The <see cref="TextWriter"/> to write iCalendar data.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="IcsWriterSettings"/> for the writer.
        /// </param>
        public static IcsWriter Create(TextWriter writer, IcsWriterSettings settings)
        {
            return new IcsWriter(writer, settings);
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="IcsWriter"/> with the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">
        ///   The <see cref="TextWriter"/> to write iCalendar data.
        /// </param>
        /// <param name="settings">
        ///   The <see cref="IcsWriterSettings"/> for the writer.
        /// </param>
        /// <remarks>
        ///   <see cref="TextWriter.NewLine"/> is set to "\r\n".
        /// </remarks>
        IcsWriter(TextWriter writer, IcsWriterSettings settings)
        {
            this.writer = writer;
            this.writer.NewLine = "\r\n";
            this.settings = settings;
        }

        /// <summary>
        ///   Closes the writer.
        /// </summary>
        /// <remarks>
        ///   If <see cref="IcsWriterSettings.CloseOutput"/> is <b>true</b>, then the underlying <see cref="TextWriter"/>
        ///   is <see cref="TextWriter.Close">closed</see>.
        /// </remarks>
        /// <exception cref="CalendarException">
        ///   When a component is not closed, i.e. missing a <see cref="WriteEndComponent"/>.
        /// </exception>
        public void Dispose()
        {
            if (startedComponentNames.Count > 0)
                throw new CalendarException(string.Format("The component '{0}' is not closed.", startedComponentNames.Peek()));

            if (writer != null && settings != null && settings.CloseOutput)
                writer.Close();
        }

        /// <summary>
        ///   Writes the <see cref="Component"/>.
        /// </summary>
        /// <param name="component">
        ///   A <see cref="Component"/> to write.
        /// </param>
        /// <remarks>
        ///   A component has the form:
        ///   <code>
        ///     BEGIN:<see cref="Component.Name"/> CRLF
        ///     (<see cref="Component.Properties"/> CRLF)* 
        ///     (<see cref="Component.Components"/> CRLF)*
        ///     END:<see cref="Component.Name"/> CRLF
        ///   </code>
        ///   A property is a <see cref="ContentLine"/> with the form: 
        ///   <code>
        ///     <see cref="ContentLine.Name"/> 
        ///       (";" <see cref="ContentLine.Parameters">parameter</see>)*
        ///       ":" <see cref="ContentLine.Value"/> 
        ///       CRLF
        ///   </code>
        ///   <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE).
        ///   </para>
        /// </remarks>
        public void Write(Component component)
        {
            WriteBeginComponent(component.Name);
            foreach (var property in component.Properties.OrderBy(p => p.Name))
                Write(property);
            foreach (var subcomponent in component.Components.OrderBy(c => c.Name))
                Write(subcomponent);
            WriteEndComponent();
        }

        /// <summary>
        ///   Write the "begin" line for the specified component name.
        /// </summary>
        /// <param name="name">
        ///   The name of a component.
        /// </param>
        /// <remarks>
        ///   The line has the form:
        ///   <code>
        ///     BEGIN:<paramref name="name"/> CRLF
        ///   </code>
        /// </remarks>
        public void WriteBeginComponent(string name)
        {
            Guard.IsNotNullOrWhiteSpace(name, "componentName");

            startedComponentNames.Push(name);
            WriteLine("BEGIN:" + name);
        }

        /// <summary>
        ///   Closes the current component and writes an "end" line.
        /// </summary>
        /// <remarks>
        ///   The line has the form:
        ///   <code>
        ///     END:<i>name</i> CRLF
        ///   </code>
        /// </remarks>
        public void WriteEndComponent()
        {
            WriteLine("END:" + startedComponentNames.Pop());
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and <see cref="string"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="string"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        /// <para>
        ///   If <paramref name="value"/> is <b>null</b>, then the content line is <b>NOT</b> written.
        /// </para>
        ///  </para>
        ///  <para>
        ///   A content line has the form: <c><paramref name="name"/>  ":" <paramref name="value"/> CRLF</c>
        ///   </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        public void WriteContent(string name, string value)
        {
            if (value != null)
            {
                var content = new ContentLine { Name = name, Value = value };
                Write(content);
            }
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and <see cref="int"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="int"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        /// <para>
        ///   If <paramref name="value"/> is <b>null</b>, then the content line is <b>NOT</b> written.
        /// </para>
        ///  </para>
        ///  <para>
        ///   A content line has the form: <c><paramref name="name"/>  ":" <paramref name="value"/> CRLF</c>
        ///   </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        public void WriteContent(string name, int value)
        {
            var content = new ContentLine { Name = name, Value = value.ToString(CultureInfo.InvariantCulture) };
            Write(content);
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and <see cref="GeoCoordinate"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="GeoCoordinate"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        /// <para>
        ///   If <paramref name="value"/> is <b>null</b>, then the content line is <b>NOT</b> written.
        /// </para>
        ///  </para>
        ///  <para>
        ///   A content line has the form: <c><paramref name="name"/>  ":" <paramref name="value"/> CRLF</c>
        ///   </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        public void WriteContent(string name, GeoCoordinate value)
        {
            if (value == null)
                return;

            var content = new ContentLine
            {
                Name = name, 
                Value = string.Format(CultureInfo.InvariantCulture, "{0};{1}", value.Latitude, value.Longitude)
            };
            Write(content);
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and values.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="values">The string values of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        /// <para>
        ///   If <paramref name="values"/> is <b>null</b>, then the content line is <b>NOT</b> written.
        /// </para>
        ///  </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        public void WriteContent(string name, IEnumerable<string> values)
        {
            if (values != null)
            {
                var content = new ContentLine { Name = name, Values = values.ToArray() };
                Write(content);
            }
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and the <see cref="Date"/>.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="Date"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        /// <para>
        ///   If <paramref name="value"/> is <b>null</b>, then the content line is <b>NOT</b> written.
        /// </para>
        ///  </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        /// <seealso cref="WriteContent(string, Date)"/>
        public void WriteContent(string name, Date? value)
        {
            if (value.HasValue)
                WriteContent(name, value.Value);
        }

        /// <summary>
        ///   Writes <see cref="ContentLine"/>(s) that contains the property name and the <see cref="Date"/> value(s).
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="Date"/> values of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// </remarks>
        public void WriteContent(string name, IEnumerable<Date> value)
        {
            if (value == null)
                return;

            foreach (var date in value)
            {
                WriteContent(name, date);
            }
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and the <see cref="Date"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="Date"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// <para>
        ///   If <paramref name="value"/> is <b>null</b>, then the content line is <b>NOT</b> written.
        /// </para>
        ///  <para>
        ///   A content line has the form: <c><paramref name="name"/>  ":" <paramref name="value"/> CRLF</c>
        ///   </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        public void WriteContent(string name, Date value)
        {
            var s = (value.IsDateOnly) // TODO: Need to set VALUE parameter
                ? value.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
                : IsoFormat(value.Value);
            var content = new ContentLine { Name = name, Value = s };
            if (!string.IsNullOrWhiteSpace(value.TimeZone))
                content.Parameters[ParameterName.TimeZoneId] = value.TimeZone;

            Write(content);
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and the <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="TimeSpan"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        /// </para>
        /// <para>
        ///   If <paramref name="value"/> does not have a value, then the content line is <b>NOT</b> written.
        /// </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        /// <seealso cref="WriteContent(string, TimeSpan)"/>
        public void WriteContent(string name, TimeSpan? value)
        {
            if (value.HasValue)
                WriteContent(name, value.Value);
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and the <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="TimeSpan"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        /// <seealso cref="WriteContent(string, TimeSpan)"/>
        public void WriteContent(string name, TimeSpan value)
        {
            var content = new ContentLine { Name = name, Value = XmlConvert.ToString(value) };
            Write(content);
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="Date"/> value of the property.</param>
        /// <remarks>
        ///   Formats the <paramref name="value"/> as an ISO Date and time.  If <see cref="DateTimeKind.Utc"/>, a "Z" is
        ///   appending to the string.
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// </remarks>
        public void WriteContent(string name, DateTime value)
        {
            var content = new ContentLine { Name = name, Value = IsoFormat(value) };
            Write(content);
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and <see cref="Text"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="Text"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// </remarks>
        public void WriteContent(string name, Text value)
        {
            if (value.Value == null)
                return;

            var content = new ContentLine { Name = name, Value = value };
            if (value.Language != null && value.Language != LanguageTag.Unspecified)
                content.Parameters[ParameterName.Language] = value.Language.Name;

            Write(content);
        }

        /// <summary>
        ///   Writes <see cref="ContentLine"/>(s) that contain the property name and each <see cref="MultilingualText"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="Text"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// </remarks>
        public void WriteContent(string name, IEnumerable<Text> value)
        {
            if (value == null)
                return;

            foreach (var text in value)
            {
                WriteContent(name, text);
            }
        }

        /// <summary>
        ///   Writes a <see cref="ContentLine"/> that contains the property name and <see cref="MailAddress"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="MailAddress"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// </remarks>
        public void WriteContent(string name, MailAddress value)
        {
            if (value == null)
                return;

            var content = new ContentLine { Name = name, Value = "mailto:" + value.Address };
            if (value.DisplayName != null)
                content.Parameters[ParameterName.CommonName] = value.DisplayName;

            Write(content);
        }

        /// <summary>
        ///   Writes <see cref="ContentLine"/>(s) that contain the property name and each <see cref="MailAddress"/> value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="values">The <see cref="MailAddress"/> value of the property.</param>
        /// <remarks>
        ///  <para>
        ///   The property name is case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <paramref name="name"/>
        ///   is converted to uppercase.
        ///  </para>
        /// </remarks>
        public void WriteContent(string name, IEnumerable<MailAddress> values)
        {
            if (values == null)
                return;

            foreach (var mailAddress in values)
            {
                WriteContent(name, mailAddress);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The <see cref="TimeSpan"/> value of the property.</param>
        public void WriteContentAsUtcOffset(string name, TimeSpan value)
        {
            var s = value == TimeSpan.Zero ? "" : value < TimeSpan.Zero ? "-" : "+";
            s += value.ToString("hhmm");
            var content = new ContentLine { Name = name, Value = s };
            Write(content);
        }

        /// <summary>
        ///   Writes the <see cref="ContentLine"/>(s) for each <see cref="IcsSerializable"/> value.
        /// </summary>
        /// <param name="values">The <see cref="IcsSerializable"/> value(s).</param>
        public void WriteContent(IEnumerable<IcsSerializable> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                WriteContent(value);
            }
        }

        /// <summary>
        ///   Writes the <see cref="ContentLine"/> for the <see cref="IcsSerializable"/> value.
        /// </summary>
        /// <param name="value">The <see cref="IcsSerializable"/> value.</param>
        public void WriteContent(IcsSerializable value)
        {
            if (value == null)
                return;

            value.WriteIcs(this);
        }

        /// <summary>
        ///   ISO format of a date time.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        static internal string IsoFormat(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);

            return dateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);   
        }

        /// <summary>
        ///   Writes the <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   A <see cref="ContentLine"/>.
        /// </param>
        /// <remarks>
        ///  <para>
        ///   The property and parameters names are case-insensitive.  However, some iCalendar readers expect upper-case.  Therefore, the <see cref="ContentLine.Name"/>
        ///   and <see cref="ContentLine.Parameters">parameter names</see> are converted to uppercase.
        ///  </para>
        ///  <para>
        ///   A content line has the form: <c><see cref="ContentLine.Name"/> *(";" <see cref="ContentLine.Parameters">parameter</see> ) ":" <see cref="ContentLine.Value"/> CRLF</c>
        ///  </para>
        ///   <para>
        ///   A long line will be split between any two characters by inserting a CRLF
        ///   immediately followed by a single linear white-space character (i.e., SPACE or HTAB).
        ///   </para>
        /// </remarks>
        public void Write(ContentLine content)
        {
            Guard.IsNotNull(content, "content");
            Guard.IsNotNull(content.Values, "content.Values");

            Write(content.Name.ToUpperInvariant());

            if (content.HasParameters)
            {
                foreach (var key in content.Parameters.AllKeys)
                {
                    if (content.Parameters.GetValues(key).All(v => string.IsNullOrWhiteSpace(v)))
                        continue;

                    Write(";");
                    Write(key.ToUpperInvariant());
                    string prefix = "=";
                    foreach (var s in content.Parameters.GetValues(key))
                    {
                        if (string.IsNullOrWhiteSpace(s))
                            continue;

                        Write(prefix);
                        bool quoteIt = s.IndexOfAny(new[] {',', ';', ':'}) >= 0;

                        // Convert newlines into escaped string ("\n").
                        var v = s.Replace("\r\n", "\n");
                        if (v.Contains('\n'))
                        {
                            quoteIt = true;
                            v = v.Replace("\n", "\\n");
                        }

                        if (quoteIt)
                            Write("\"");
                        Write(v);
                        if (quoteIt)
                            Write("\"");
                        prefix = ",";
                    }
                }
            }

            Write(":");
            for (int i = 0; i < content.Values.Length; ++i)
            {
                if (i != 0)
                    Write(",");
                WriteEscaped(content.Values[i]);
            }
            WriteLine();
        }

        /// <summary>
        ///   Writes the <see cref="string"/>
        /// </summary>
        /// <param name="s">The string to write.</param>
        /// <remarks>
        ///   If <see cref="IcsWriterSettings.OctetsPerLine"/> would be exceeded, then a line fold (CRLF + SP) is injected.
        /// </remarks>
        void Write(string s)
        {
            var octets = utf8.GetByteCount(s);
            if (octets + lineOctets <= settings.OctetsPerLine)
            {
                writer.Write(s);
                lineOctets += octets;
                return;
            }

            var chars = s.ToCharArray();
            for(int i = 0; i < chars.Length; ++i)
            {
                octets = utf8.GetByteCount(chars, i, 1);
                if (octets + lineOctets > settings.OctetsPerLine)
                {
                    writer.WriteLine();
                    writer.Write(' ');
                    lineOctets = 1;
                }
                writer.Write(chars[i]);
                lineOctets += octets;
            }
        }

        void WriteEscaped(string s)
        {
            s = s.Replace(@"\", @"\\");
            s = s.Replace(@",", @"\,");
            s = s.Replace("\r\n", @"\n");
            Write(s);
        }

        void WriteLine(string s)
        {
            Write(s);
            WriteLine();
        }
        void WriteLine()
        {
            writer.WriteLine();
            lineOctets = 0;
        }
    }
}
