using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   An address vCard value.
    /// </summary>
    public class VCardAddress : VCardValue
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCardAddress"/> class.
        /// </summary>
        public VCardAddress()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCardAddress"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the property parameters.
        /// </param>
        public VCardAddress(ContentLine content)
            : base(content)
        {
            if (content.HasParameters)
            {
                GeographicPositionUrl = content.Parameters["geo"];
                Label = content.Parameters["label"];
            }

            var parts = content.Value.Split(';');
            PostOfficeBox = parts.Length > 0 ? parts[0] : null;
            ExtendedAddress = parts.Length > 1 ? parts[1] : null;
            StreetAddress = parts.Length > 2 ? parts[2] : null;
            Locality = parts.Length > 3 ? parts[3] : null;
            Region = parts.Length > 4 ? parts[4] : null;
            PostalCode = parts.Length > 5 ? parts[5] : null;
            Country = parts.Length > 6 ? parts[6] : null;
        }

        /// <summary>
        ///   URL to a geographic position; see <see href="http://tools.ietf.org/html/rfc5870">RFC 5870</see>.
        /// </summary>
        public string GeographicPositionUrl { get; set; }

        /// <summary>
        ///   Delivery address label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///   Post office box.
        /// </summary>
        public string PostOfficeBox { get; set; }

        /// <summary>
        ///   Apartment or suite.
        /// </summary>
        public string ExtendedAddress { get; set; }

        /// <summary>
        ///   Street address.
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        ///   City.
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        ///   Region or state.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        ///   Postal code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        ///   Full name of the country.
        /// </summary>
        public string Country { get; set; }

        /// <inheritdoc />
        public override ContentLine ToContentLine(ContentLine content = null)
        {
            content = base.ToContentLine(content);
            if (GeographicPositionUrl != null)
                content.Parameters["geo"] = GeographicPositionUrl;
            if (Label != null)
                content.Parameters["label"] = Label;

            var s = new StringBuilder();
            var empty = new List<string>(0);
            s.Append(PostOfficeBox);
            s.Append(';');
            s.Append(ExtendedAddress);
            s.Append(';');
            s.Append(StreetAddress);
            s.Append(';');
            s.Append(Locality);
            s.Append(';');
            s.Append(Region);
            s.Append(';');
            s.Append(PostalCode);
            s.Append(';');
            s.Append(Country);
            content.Value = s.ToString();

            return content;
        }

    }
}
