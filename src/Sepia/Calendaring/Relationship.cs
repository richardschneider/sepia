using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Identifies the relationship between two calendar components.
    /// </summary>
    /// <seealso cref="RelationshipReference"/>
    public class Relationship : Tag
    {
        /// <summary>
        ///   Parent relationship.
        /// </summary>
        public static Relationship Parent = new Relationship { Name = "parent" };

        /// <summary>
        ///   Child relationship.
        /// </summary>
        public static Relationship Child = new Relationship { Name = "child" };

        /// <summary>
        ///   Sibling relationship.
        /// </summary>
        public static Relationship Sibling = new Relationship { Name = "sibling" };

        /// <summary>
        ///   Creates a new instance of the <see cref="UserRole"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public Relationship()
        {
            Authority = "ietf:rfc5545";
        }
    }
}
