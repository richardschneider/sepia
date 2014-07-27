using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Represents a date and/or time.
    /// </summary>
    public class VCardDate : VCardValue
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCardDate"/> class.
        /// </summary>
        public VCardDate()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCardDate"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the property parameters.
        /// </param>
        public VCardDate(ContentLine content)
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

    }
}
