using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Represents some time(s) that are either free or busy in a calendar.  
    /// </summary>
    /// <remarks>
    ///   <b>TimeSlot</b> is used by the <see cref="VFreeBusy"/> calendar component.
    /// </remarks>
    public class TimeSlot : IcsSerializable
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="TimeSlot"/> class with the default value.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Usage"/> to <see cref="TimeUsage.Busy"/>.
        /// </remarks>
        public TimeSlot()
        {
            Usage = TimeUsage.Busy;
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="TimeSlot"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the FreeBusyTime.
        /// </param>
        public TimeSlot(ContentLine content)
            : this()
        {
            ReadIcs(content);
        }

        /// <summary>
        ///   Identifies how the <see cref="TimeRanges"/> are used.
        /// </summary>
        /// <value>
        ///   A <see cref="TimeUsage"/> tag.  The default value is <see cref="TimeUsage.Busy"/>
        /// </value>
        /// <seealso cref="IsFree"/>
        /// <seealso cref="IsBusy"/>
        public TimeUsage Usage { get; set; }

        /// <summary>
        ///   The times that are either <see cref="IsFree">free</see> or
        ///   <see cref="IsBusy">busy</see>.
        /// </summary>
        /// <value>
        ///   The <see cref="Sepia.TimeRange"/> that is either <see cref="IsFree">free</see> or
        ///   <see cref="IsBusy">busy</see>.
        /// </value> 
        public List<TimeRange> TimeRanges { get; set; }

        /// <summary>
        ///   Determines if the <see cref="TimeRanges"/> are free or not.
        /// </summary>
        /// <returns>
        ///   <b>true</b> if <see cref="Usage"/> equals <see cref="TimeUsage.Free"/>; otherwise, <b>false</b>.
        /// </returns>
        /// <seealso cref="IsBusy"/>
        /// <seealso cref="Usage"/>
        public bool IsFree
        {
            get { return Usage == TimeUsage.Free; }
        }

        /// <summary>
        ///   Determines if the <see cref="TimeRanges"/> are busy or not.
        /// </summary>
        /// <returns>
        ///   <b>true</b> if <see cref="Usage"/> does not equal <see cref="TimeUsage.Free"/>; otherwise, <b>false</b>.
        /// </returns>
        /// <seealso cref="IsFree"/>
        /// <seealso cref="Usage"/>
        public bool IsBusy
        {
            get { return Usage != TimeUsage.Free; }
        }


        /// <inheritdoc />
        public void ReadIcs(IcsReader reader)
        {
            Guard.IsNotNull(reader, "reader");

            ReadIcs(reader.ReadContentLine());
        }

        void ReadIcs(ContentLine content)
        {
            Guard.IsNotNull(content, "content");
            Guard.Require(content.Name.Equals(PropertyName.FreeBusy, StringComparison.InvariantCultureIgnoreCase), "content", "Expected an FREEBUSY content line.");

            if (content.HasParameters)
            {
                var usage = content.Parameters["fbtype"];
                if (usage != null)
                    Usage = new TimeUsage { Name = usage };
            }

            TimeRanges = content.ToTimeRangeEnumerable().ToList();
        }

        /// <inheritdoc />
        public void WriteIcs(IcsWriter writer)
        {
            Guard.IsNotNull(writer, "writer");

            const string form = "yyyyMMdd'T'HHmmss'Z'";
            var content = new ContentLine { Name = PropertyName.FreeBusy };
            content.Parameters["FBTYPE"] = Usage.Name.ToUpperInvariant();
            content.Values = TimeRanges
                .Select(t => t.StartsOn.UtcDateTime.ToString(form) + "/" + t.EndsOn.UtcDateTime.ToString(form))
                .ToArray();

            writer.Write(content);
        }

        /// <summary>
        ///   Converts the <see cref="CalendarAttachment"/> to a RFC 5545 line.
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
                this.WriteIcs(writer);
            }
            return s.ToString().TrimEnd('\r', '\n');
        }

    }
}
