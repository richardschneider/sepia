using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Identifies the participation status of a calendar user.
    /// </summary>
    public class ParticipationStatus : Tag
    {
        /// <summary>
        ///   Needs action.
        /// </summary>
        public static ParticipationStatus NeedsAction = new ParticipationStatus { Name = "needs-action" };

        /// <summary>
        ///   Accepted.
        /// </summary>
        public static ParticipationStatus Accepted = new ParticipationStatus { Name = "accepted" };

        /// <summary>
        ///   Declined.
        /// </summary>
        public static ParticipationStatus Declined = new ParticipationStatus { Name = "declined" };

        /// <summary>
        ///   Tentatively accepted.
        /// </summary>
        public static ParticipationStatus Tentative = new ParticipationStatus { Name = "tentative" };

        /// <summary>
        ///   Delegated.
        /// </summary>
        public static ParticipationStatus Delegated = new ParticipationStatus { Name = "delegated" };

        /// <summary>
        ///   Completed.
        /// </summary>
        public static ParticipationStatus Completed = new ParticipationStatus { Name = "completed" };

        /// <summary>
        ///   In process.
        /// </summary>
        public static ParticipationStatus InProcess = new ParticipationStatus { Name = "in-process" };

        /// <summary>
        ///   Creates a new instance of the <see cref="ParticipationStatus"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public ParticipationStatus()
        {
            Authority = "ietf:rfc5545";
        }
}
}
