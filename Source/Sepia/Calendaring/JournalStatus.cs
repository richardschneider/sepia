using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   The overall status or confirmation of a <see cref="VJournal"/>.
    /// </summary>
    public class JournalStatus : Tag
    {
        /// <summary>
        ///   The <see cref="VJournal"/> is a draft.
        /// </summary>
        public static JournalStatus Draft = new JournalStatus { Name = "draft" };

        /// <summary>
        ///   The <see cref="VJournal"/> is final.
        /// </summary>
        public static JournalStatus Final = new JournalStatus { Name = "final" };

        /// <summary>
        ///   The <see cref="VJournal"/> is cancelled.
        /// </summary>
        public static JournalStatus Cancelled = new JournalStatus { Name = "cancelled" };

        /// <summary>
        ///   Creates a new instance of the <see cref="JournalStatus"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public JournalStatus()
        {
            Authority = "ietf:rfc5545";
        }

    }
}
