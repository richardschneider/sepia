using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Identifies the type of action to take when a <see cref="VAlarm"/> is triggered.
    /// </summary>
    /// <seealso cref="VAlarm"/>
    public class AlarmAction : Tag
    {
        /// <summary>
        ///   Play a sound.
        /// </summary>
        public static AlarmAction Audio = new AlarmAction { Name = "audio" };

        /// <summary>
        ///   Display some text.
        /// </summary>
        public static AlarmAction Display = new AlarmAction { Name = "display" };

        /// <summary>
        ///   Send an email.
        /// </summary>
        public static AlarmAction Email = new AlarmAction { Name = "email" };

        /// <summary>
        ///   Creates a new instance of the <see cref="AlarmAction"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public AlarmAction()
        {
            Authority = "ietf:rfc5545";
        }
    }
}
