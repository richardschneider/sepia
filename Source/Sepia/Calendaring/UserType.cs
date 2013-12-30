using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Identifies the type of a calendar user.
    /// </summary>
    public class UserType : Tag
    {
        /// <summary>
        ///   An individual.
        /// </summary>
        public static UserType Individual = new UserType { Name = "individual" };

        /// <summary>
        ///   A group of individuals.
        /// </summary>
        public static UserType Group = new UserType { Name = "group" };

        /// <summary>
        ///   A physical resource.
        /// </summary>
        public static UserType Resource = new UserType { Name = "resource" };

        /// <summary>
        ///   A room resource.
        /// </summary>
        public static UserType Room = new UserType { Name = "room" };

        // <summary>
        //   Unknown.
        // </summary>
        // TODO: public static UserType Unknown = new UserType { Name = "unknown" };

        /// <summary>
        ///   Creates a new instance of the <see cref="UserType"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public UserType()
        {
            Authority = "ietf:rfc5545";
        }
}
}
