using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Provides a method of capturing the scope of the access the calendar owner intends for information within an individual
    ///   calendar entry.
    /// </summary>
    public class AccessClassification : Tag
    {
        /// <summary>
        ///   Everyone has access to the information.
        /// </summary>
        public static AccessClassification Public = new AccessClassification { Name = "public" };

        /// <summary>
        ///   Only the owner has access to the information.
        /// </summary>
        public static AccessClassification Private = new AccessClassification { Name = "private" };

        /// <summary>
        ///   Only some people have access to the information.
        /// </summary>
        public static AccessClassification Confidential = new AccessClassification { Name = "confidential" };

        /// <summary>
        ///   Creates a new instance of the <see cref="AccessClassification"/> class with the default values.
        /// </summary>
        /// <remarks>
        ///   Sets <see cref="Tag.Authority"/> to "ietf:rfc5545".
        /// </remarks>
        public AccessClassification()
        {
            Authority = "ietf:rfc5545";
        }

    }
}
