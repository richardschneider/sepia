using System;
using System.Collections.Generic;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Identifies the base time that is used to determine when a <see cref="VAlarm"/> is triggered.  The actual trigger time is the
    ///   base time plus the <see cref="VAlarm.TriggerDuration"/>.
    /// </summary>
    /// <seealso cref="VAlarm"/>
    public class TriggerEdge : Tag
    {
        /// <summary>
        ///   The base time is the start of the <see cref="VEvent"/> or <see cref="VTodo"/>.
        /// </summary>
        public static TriggerEdge Start = new TriggerEdge { Name = "start" };

        /// <summary>
        ///   The base time is the start of the <see cref="VEvent"/> or <see cref="VTodo"/>.
        /// </summary>
        public static TriggerEdge End = new TriggerEdge { Name = "end" };

        /// <summary>
        ///   Creates a new instance of the <see cref="TriggerEdge"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public TriggerEdge()
        {
            Authority = "ietf:rfc5545";
        }
    }
}
