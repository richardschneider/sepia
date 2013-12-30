using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Represents a document attached to calendar component.  
    /// </summary>
    /// <remarks>
    ///   The attachment is either a URI to the document or the binary content of the document.
    /// </remarks>
    public class CalendarAttachment : IcsSerializable
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="CalendarAttachment"/> class.
        /// </summary>
        public CalendarAttachment()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="CalendarAttachment"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the CalendarAttachment.
        /// </param>
        public CalendarAttachment(ContentLine content) : this()
        {
            ReadIcs(content);
        }

        /// <summary>
        ///   The URI to the document.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        ///   The binary content of the document.
        /// </summary>
        /// <remarks>
        ///   The <see cref="ContentType"/> can be used to describe the format of the content.
        /// </remarks>
        public byte[] Content { get; set; }

        /// <summary>
        ///   The MIME type that describes the <see cref="Content"/>.
        /// </summary>
        public string ContentType { get; set; }

        /// <inheritdoc />
        public void ReadIcs(IcsReader reader)
        {
            Guard.IsNotNull(reader, "reader");

            ReadIcs(reader.ReadContentLine());
        }

        void ReadIcs(ContentLine content)
        {
            Guard.IsNotNull(content, "content");
            Guard.Check(content.Name.Equals(PropertyName.Attachment, StringComparison.InvariantCultureIgnoreCase), "content", "Expected an attachment content line.");

            ContentType = content.Parameters[ParameterName.FormatType];
            var valueType = content.Parameters["value"];
            if (valueType != null && valueType.Equals("binary", StringComparison.InvariantCultureIgnoreCase))
            {
                switch (content.Parameters[ParameterName.InlineEncoding].ToLowerInvariant())
                {
                    case "base64":
                        Content = Convert.FromBase64String(content.Value);
                        break;
                    default:
                        throw new CalendarException(string.Format("The encoding '{0}' is not known.", content.Parameters[ParameterName.InlineEncoding]));
                }
            }
            else
            {
                Uri = content.Value;
            }
        }

        /// <inheritdoc />
        public void WriteIcs(IcsWriter writer)
        {
            Guard.IsNotNull(writer, "writer");

            var content = new ContentLine { Name = PropertyName.Attachment };
            if (ContentType != null)
                content.Parameters[ParameterName.FormatType] = ContentType;

            if (Uri != null)
                content.Value = Uri;
            else if (Content != null)
            {
                content.Parameters[ParameterName.InlineEncoding] = "BASE64";
                content.Parameters["VALUE"] = "BINARY";
                content.Value = Convert.ToBase64String(Content);
            }

            writer.Write(content);
        }

        /// <summary>
        ///   Converts the <see cref="CalendarAttachment"/> to a RFC 5545 line.
        /// </summary>
        /// <returns>
        ///   The iCalendar representation.
        /// </returns>
        /// <remarks>
        ///   Line folds are not applied.
        /// </remarks>
        public override string ToString()
        {
            var settings = new IcsWriterSettings() { OctetsPerLine = Int32.MaxValue };
            var s = new StringWriter();
            using (var writer = IcsWriter.Create(s, settings))
            {
                this.WriteIcs(writer);
            }
            return s.ToString().TrimEnd('\r', '\n');
        }

    }
}
