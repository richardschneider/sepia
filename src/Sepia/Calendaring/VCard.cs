using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sepia.Calendaring
{

    /// <summary>
    ///   Represents information about individuals and other entities (vCard).
    /// </summary>
    public class VCard
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCard"/> class with the default values.
        /// </summary>
        /// 
        public VCard()
        {
            Addresses = new List<VCardAddress>();
            Emails = new List<VCardString>(0);
            FormattedNames = new List<VCardText>(0);
            GeographicPositions = new List<VCardUri>(0);
            Languages = new List<VCardString>(0);
            Names = new List<VCardName>(0);
            NickNames = new List<VCardText>(0);
            Photos = new List<VCardUri>(0);
            ProductId = this.GetType().Namespace;
            Sources = new List<VCardUri>(0);
            Telephones = new List<VCardString>(0);
            Timezones = new List<VCardString>(0);
            Version = "4.0";
        }

        // TODO: ANNIVERSARY
        // TODO: IMPP

        /// <summary>
        ///   Delivery address(es) for the entity;
        /// </summary>
        public List<VCardAddress> Addresses { get; set; }

        /// <summary>
        ///   The birth date of the individual.
        /// </summary>
        public VCardDate BirthDate { get; set; }

        /// <summary>
        ///   Email address(es).
        /// </summary>
        public List<VCardString> Emails { get; set; }

        /// <summary>
        ///   Gender identity of the individual..
        /// </summary>
        /// <value>
        ///   A single letter.  M stands for "male", F stands for "female", 
        ///   O stands for "other", N stands for "none or not applicable", U stands for "unknown".
        /// </value>
        public string Gender { get; set; }

        /// <summary>
        ///   URL to a geographic position(s); see <see href="http://tools.ietf.org/html/rfc5870">RFC 5870</see>.
        /// </summary>
        public List<VCardUri> GeographicPositions { get; set; }

        /// <summary>
        ///   Languages spoken by the entity.
        /// </summary>
        public List<VCardString> Languages { get; set; }

        /// <summary>
        ///   The kind of object the vCard represents.
        /// </summary>
        /// <value>
        ///   "individual", "group", "org" or "location".
        /// </value>
        public string Kind { get; set; }

        /// <summary>
        ///   Formatted text corresponding to the name of the entity the vCard represents.
        /// </summary>
        public List<VCardText> FormattedNames { get; set; }

        /// <summary>
        ///   Name components of the entity.
        /// </summary>
        public List<VCardName> Names { get; set; }

        /// <summary>
        ///   Nickname(s) of the entity.
        /// </summary>
        public List<VCardText> NickNames { get; set; }

        /// <summary>
        ///   URL(s) to the image(s) of the entity.
        /// </summary>
        public List<VCardUri> Photos { get; set; }

        /// <summary>
        ///   Specifies the identifier for the product that created the vCard.
        /// </summary>
        /// <value>
        ///   The default value is "Sepia.Calendaring".
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        ///   URL(s) to the source of directory information.
        /// </summary>
        public List<VCardUri> Sources { get; set; }

        /// <summary>
        ///   Telephone number(s).
        /// </summary>
        public List<VCardString> Telephones { get; set; }

        /// <summary>
        ///   Timezone(s) of the entity.
        /// </summary>
        public List<VCardString> Timezones { get; set; }

        /// <summary>
        ///   Version of the vCard specification used to format this vCard.
        /// </summary>
        /// <value>
        ///   The default value is "4.0".
        /// </value>
        public string Version { get; set; }

        /// <summary>
        ///   The vCard representation.
        /// </summary>
        /// <seealso cref="WriteIcs"/>
        public override string ToString()
        {
            var ics = new StringWriter();
            using (var writer = IcsWriter.Create(ics))
            {
                this.WriteIcs(writer);
            }
            return ics.ToString();
        }

        /// <summary>
        ///   Read the component properties from the <see cref="IcsReader"/>.
        /// </summary>
        /// <param name="reader">
        ///   The <see cref="IcsReader"/> containing the content of the component.
        /// </param>
        public void ReadIcs(IcsReader reader)
        {
            Guard.IsNotNull(reader, "reader");

            // Must start with "BEGIN:VCARD".
            ContentLine content = reader.ReadContentLine();
            if (content == null)
                throw new CalendarException("Unexpected end of file.");
            if (!(content.Name.ToLowerInvariant() == "begin" && content.Value.ToLowerInvariant() == Component.Names.Card.ToLowerInvariant()))
                throw new CalendarException(string.Format("Expected 'BEGIN:VCARD' not '{0}'.", content));

            // Process the content.
            while (null != (content = reader.ReadContentLine()))
            {
                switch (content.Name.ToLowerInvariant())
                {
                    case "end":
                        if (!content.Value.Equals(Component.Names.Card, StringComparison.InvariantCultureIgnoreCase))
                            throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", Component.Names.Card, content));
                        return;
                    case "adr": Addresses.Add(new VCardAddress(content)); break;
                    case "bday": BirthDate = new VCardDate(content); break;
                    case "email": Emails.Add(new VCardString(content)); break;
                    case "fn": FormattedNames.Add(new VCardText(content)); break;
                    case "gender": Gender = content.Value; break;
                    case "geo": GeographicPositions.Add(new VCardUri(content)); break;
                    case "lang": Languages.Add(new VCardString(content)); break;
                    case "kind": Kind = content.Value; break;
                    case "prodid": ProductId = content.Value; break;
                    case "n": Names.Add(new VCardName(content)); break;
                    case "nickname": NickNames.Add(new VCardText(content)); break;
                    case "photo": Photos.Add(new VCardUri(content)); break;
                    case "source": Sources.Add(new VCardUri(content)); break;
                    case "tel": Telephones.Add(new VCardString(content)); break;
                    case "tz": Timezones.Add(new VCardString(content)); break;
                    case "version": Version = content.Value; break;
                }
            }

            throw new CalendarException("Expected 'END:VCARD' not end of file.");
        }

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(Component.Names.Card);

            // Properties
            ics.WriteContent("version", Version);
            ics.WriteContent("prodid", ProductId);
            ics.WriteContent("kind", Kind);
            foreach (var source in Sources)
            {
                ics.Write(source.ToContentLine(new ContentLine { Name = "source" }));
            }

            foreach (var fn in FormattedNames)
            {
                ics.Write(fn.ToContentLine(new ContentLine { Name = "fn" }));
            }
            ics.WriteContent("gender", Gender);
            if (BirthDate != null)
                ics.Write(BirthDate.ToContentLine(new ContentLine { Name = "bday" }));

            foreach (var name in Names)
            {
                ics.Write(name.ToContentLine(new ContentLine { Name = "n" }));
            }
            foreach (var nickname in NickNames)
            {
                ics.Write(nickname.ToContentLine(new ContentLine { Name = "nickname" }));
            }
            foreach (var email in Emails)
            {
                ics.Write(email.ToContentLine(new ContentLine { Name = "email" }));
            }
            foreach (var tel in Telephones)
            {
                ics.Write(tel.ToContentLine(new ContentLine { Name = "tel" }));
            }
            foreach (var lang in Languages)
            {
                ics.Write(lang.ToContentLine(new ContentLine { Name = "lang" }));
            }
            foreach (var tz in Timezones)
            {
                ics.Write(tz.ToContentLine(new ContentLine { Name = "tz" }));
            }
            foreach (var geo in GeographicPositions)
            {
                ics.Write(geo.ToContentLine(new ContentLine { Name = "geo" }));
            }
            foreach (var address in Addresses)
            {
                ics.Write(address.ToContentLine(new ContentLine { Name = "adr" }));
            }
            foreach (var photo in Photos)
            {
                ics.Write(photo.ToContentLine(new ContentLine { Name = "photo" }));
            }
            ics.WriteEndComponent();
       }

    }
}
