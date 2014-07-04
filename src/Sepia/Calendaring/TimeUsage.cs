using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Identifies how a <see cref="TimeRange"/> is used.
    /// </summary>
    /// <seealso cref="TimeSlot"/>
    public class TimeUsage : Tag // TODO: rename TimeUsage
    {
        /// <summary>
        ///   The time is free because nothing is scheduled.
        /// </summary>
        public static TimeUsage Free = new TimeUsage { Name = "free" };

        /// <summary>
        ///   The time is busy because one or more events have been scheduled.
        /// </summary>
        public static TimeUsage Busy = new TimeUsage { Name = "busy" };

        /// <summary>
        ///   The time is busy and unavailable for scheduling.
        /// </summary>
        public static TimeUsage Unavailable = new TimeUsage { Name = "busy-unavailable" };

        /// <summary>
        ///   The time is busy because one or more events have been tentatively scheduling.
        /// </summary>
        public static TimeUsage Tentative = new TimeUsage { Name = "busy-tentative" };

        /// <summary>
        ///   Creates a new instance of the <see cref="TimeUsage"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public TimeUsage()
        {
            Authority = "ietf:rfc5545";
        }
    }
}
