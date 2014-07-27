using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   A textual vCard value.
    /// </summary>
    public class VCardText : VCardValue
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCardText"/> class.
        /// </summary>
        public VCardText()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCardText"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the property parameters.
        /// </param>
        public VCardText(ContentLine content)
            : base(content)
        {
            Value = content.ToText();
        }

        /// <summary>
        ///   The text value of the property.
        /// </summary>
        public Text Value { get; set; }

        /// <inheritdoc />
        public override ContentLine ToContentLine(ContentLine content = null)
        {
            content = base.ToContentLine(content);
            content.Value = Value.Value;
            if (Value.Language != null && Value.Language != LanguageTag.Unspecified)
                content.Parameters[ParameterName.Language] = Value.Language.Name;

            return content;
        }

        /// <summary>
        ///   Implicit casting to a <see cref="string"/>.
        /// </summary>
        /// <param name="text">
        ///   The <see cref="Text"/> to return as a <see cref="string"/>.
        /// </param>
        public static implicit operator string(VCardText text)
        {
            if (text == null)
                return null;

            return text.Value.Value;
        }

        /// <summary>
        ///   Implicit casting from a <see cref="string"/>.
        /// </summary>
        public static implicit operator VCardText(string text)
        {
            return new VCardText { Value = new Text(LanguageTag.Unspecified, text) };
        }

    }
}
