using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   A Uniform Resource Identifier vCard value.
    /// </summary>
    public class VCardUri : VCardValue
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCardUri"/> class.
        /// </summary>
        public VCardUri()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCardUri"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the property parameters.
        /// </param>
        public VCardUri(ContentLine content)
            : base(content)
        {
            Value = content.Value;
        }

        /// <summary>
        ///   The URL value of the property.
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc />
        public override ContentLine ToContentLine(ContentLine content = null)
        {
            content = base.ToContentLine(content);
            content.Value = Value;

            return content;
        }

        /// <summary>
        ///   Implicit casting to a <see cref="string"/>.
        /// </summary>
        /// <param name="uri">
        ///   The <see cref="VCardUri"/> to return as a <see cref="string"/>.
        /// </param>
        public static implicit operator string(VCardUri uri)
        {
            if (uri == null)
                return null;

            return uri.Value;
        }

        /// <summary>
        ///   Implicit casting from a <see cref="string"/>.
        /// </summary>
        public static implicit operator VCardUri(string url)
        {
            return new VCardUri { Value = url };
        }

    }
}
