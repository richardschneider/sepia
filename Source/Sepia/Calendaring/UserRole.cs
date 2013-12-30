using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Identifies the participation role a calendar user.
    /// </summary>
    /// <seealso cref="ParticipationStatus"/>
    public class UserRole : Tag
    {
        /// <summary>
        ///   Participation is required.
        /// </summary>
        public static UserRole Required = new UserRole { Name = "req-participant" };

        /// <summary>
        ///   Participation is optional.
        /// </summary>
        public static UserRole Optional = new UserRole { Name = "opt-participant" };

        /// <summary>
        ///   Copied for information purposes only.
        /// </summary>
        public static UserRole NonParticipant = new UserRole { Name = "non-participant" };

        /// <summary>
        ///   Creates a new instance of the <see cref="UserRole"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public UserRole()
        {
            Authority = "ietf:rfc5545";
        }
    }
}
