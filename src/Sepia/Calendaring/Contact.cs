using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    using Sepia.Calendaring.Serialization;

    /// <summary>
    ///   Textual contact information.
    /// </summary>
    /// <remarks>
    ///   An <see cref="Uri">alternative representation</see> for the <b>Contact</b> can also be specified that refers to a URI pointing to an
    ///   alternate form, such as a vCard [RFC2426], for the contact information.
    /// </remarks>
    public class Contact : IcsSerializable
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="Contact"/> class.
        /// </summary>
        public Contact()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="Contact"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the contact.
        /// </param>
        public Contact(ContentLine content) : this()
        {
            Guard.IsNotNull(content, "content");
            Guard.Require(content.Name.Equals("CONTACT", StringComparison.InvariantCultureIgnoreCase), "content", "Expected a CONTACT content line.");

            Text = content.ToText();
            Uri = content.Parameters[ParameterName.AlternativeRepresentation];
        }

        /// <summary>
        ///   Textual representation of the contact.
        /// </summary>
        public Text Text { get; set; }

        /// <summary>
        ///  An alternate representation for the contact at a URI.
        /// </summary>
        /// <value>
        ///   The default value is <b>null</b>.
        /// </value>
        public string Uri { get; set; }


        /// <inheritdoc />
        public void ReadIcs(IcsReader reader) // TODO
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void WriteIcs(IcsWriter writer)
        {
            var content = new ContentLine { Name = "contact", Value = Text };
            if (Uri != null)
                content.Parameters[ParameterName.AlternativeRepresentation] = Uri;
            writer.Write(content);
        }
    }
}
