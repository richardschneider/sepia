using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   The overall status or confirmation of a <see cref="VEvent"/>.
    /// </summary>
    public class EventStatus : Tag
    {
        /// <summary>
        ///   The event may or may not happen.
        /// </summary>
        public static EventStatus Tentative = new EventStatus { Name = "tentative" };

        /// <summary>
        ///   The event is definite.
        /// </summary>
        public static EventStatus Confirmed = new EventStatus { Name = "confirmed" };

        /// <summary>
        ///   The event is cancelled.
        /// </summary>
        public static EventStatus Cancelled = new EventStatus { Name = "cancelled" };

        /// <summary>
        ///   Creates a new instance of the <see cref="EventStatus"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public EventStatus()
        {
            Authority = "ietf:rfc5545";
        }

    }
}
