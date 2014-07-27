using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Represents the standard parameters for a VCard value.
    /// </summary>
    /// <remark>
    ///   Derived classes, will specialise the Value.
    /// </remark>
    public class VCardValue
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCardValue"/> class.
        /// </summary>
        public VCardValue()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCardValue"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the property parameters.
        /// </param>
        public VCardValue(ContentLine content)
            : this()
        {
            Guard.IsNotNull(content, "content");
            if (!content.HasParameters)
                return;

            var p = content.Parameters;
            var v = p[ParameterName.Preference];
            if (v != null)
                Preference = int.Parse(v, NumberStyles.Integer, CultureInfo.InvariantCulture);
            AlternativeId = p[ParameterName.AlternativeId];
            Id = p[ParameterName.PropertyId];
            Type = p[ParameterName.Type];
        }

        /// <summary>
        ///   Writes the parameters to the <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> to write the parameters into.  if <b>null</b> a
        ///   new <b>ContentLine</b> is created.
        /// </param>
        public virtual ContentLine ToContentLine(ContentLine content = null)
        {
            content = content ?? new ContentLine();

            if (Preference.HasValue)
                content.Parameters[ParameterName.Preference] = Preference.Value.ToString(CultureInfo.InstalledUICulture);
            if (AlternativeId != null) 
                content.Parameters[ParameterName.AlternativeId] = AlternativeId;
            if (Id != null)
                content.Parameters[ParameterName.PropertyId] = Id;
            if (Type != null)
                content.Parameters[ParameterName.Type] = Type;

            return content;
        }

        /// <summary>
        ///   The preference order assigned by the author.
        /// </summary>
        /// <value>
        ///   Values are between 1 and 100; with 1 being the highest.
        /// </value>
        public int? Preference { get; set; }

        /// <summary>
        ///   The ID for an alternative representation of a property.
        /// </summary>
        public string AlternativeId { get; set; }

        /// <summary>
        ///   Property identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///   Class characteristics of the associated property.
        /// </summary>
        public string Type { get; set; }
    }
}
