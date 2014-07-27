using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   A vCard string value.
    /// </summary>
    public class VCardString : VCardValue
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCardString"/> class.
        /// </summary>
        public VCardString()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCardString"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the property parameters.
        /// </param>
        public VCardString(ContentLine content)
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
        /// <param name="value">
        ///   The <see cref="VCardUri"/> to return as a <see cref="string"/>.
        /// </param>
        public static implicit operator string(VCardString value)
        {
            if (value == null)
                return null;

            return value.Value;
        }

        /// <summary>
        ///   Implicit casting from a <see cref="string"/>.
        /// </summary>
        public static implicit operator VCardString(string s)
        {
            return new VCardString { Value = s };
        }

    }
}
