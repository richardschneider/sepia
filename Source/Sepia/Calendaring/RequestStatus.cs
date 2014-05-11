using Sepia.Calendaring.Serialization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   The status of a scheduling request.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   It consists of a short <see cref="Code"/>, a longer <see cref="Description"/>, 
    ///   and an optional <see cref="RelatedData"/>.
    ///  </para>
    ///  <para>
    ///   A <b>RequestStatus</b> is used by <see cref="VEvent"/>, <see cref="VTodo"/>, <see cref="VJournal"/> and 
    ///   <see cref="VFreeBusy"/> to indicate the status of a scheduling request.
    /// </para>
    /// </remarks>
    public class RequestStatus : IcsSerializable
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="RequestStatus"/> class.
        /// </summary>
        public RequestStatus()
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="RequestStatus"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the request status.
        /// </param>
        public RequestStatus(ContentLine content)
        {
            ReadIcs(content);
        }

        /// <summary>
        ///   A numerical hierarchy of status codes intended for applications.
        /// </summary>
        /// <value>
        ///   Each level is represented by an integer and levels are separated by a full stop ('.'); such
        ///   as "2.1".
        /// </value>
        public string Code { get; set; }

        /// <summary>
        ///   A human readable description of the status.
        /// </summary>
        public Text Description { get; set; }

        /// <summary>
        ///   Optional data that is related to the status.
        /// </summary>
        public string RelatedData { get; set; }

        /// <summary>
        ///   Indicates that the request has been initially processed but that completion is pending. 
        /// </summary>
        public bool IsPreliminarySuccess { get { return Code.StartsWith("1."); } }

        /// <summary>
        ///   Indicates that the request was successfully completed. 
        /// </summary>
        public bool IsSuccess { get { return Code.StartsWith("2."); } }

        /// <summary>
        ///   Indicates that the request was not successfully completed and a syntax or a semantic error
        ///   exists in the client request. 
        /// </summary>
        public bool IsClientError { get { return Code.StartsWith("3."); } }

        /// <summary>
        ///   Indicates that the request was not successfully completed and an error
        ///   occurred in the calendaring and scheduling service,  not directly related to the request itself.
        /// </summary>
        public bool IsServerError { get { return Code.StartsWith("4."); } }

        /// <summary>
        ///   Converts the <see cref="RequestStatus"/> to a RFC 5545 line.
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

        /// <inheritdoc />
        public void ReadIcs(IcsReader reader)
        {
            Guard.IsNotNull(reader, "reader");

            ReadIcs(reader.ReadContentLine());
        }

        void ReadIcs(ContentLine content)
        {
            Guard.IsNotNull(content, "content");
            Guard.Require(content.Name.Equals("REQUEST-STATUS", StringComparison.InvariantCultureIgnoreCase), "content", "Expected a REQUEST-STATUS content line.");

            var parts = content.Value.Split(';');
            if (parts.Length > 0)
                Code = parts[0].Trim();
            if (parts.Length > 1)
                Description = new Text(content.Parameters[ParameterName.Language], parts[1].Trim());
            if (parts.Length > 2)
            {
                RelatedData = parts[2].Trim();
                for (int i = 3; i < parts.Length; ++i)
                {
                    RelatedData += ";" + parts[i].Trim();
                }
            }
        }

        /// <inheritdoc />
        public void WriteIcs(IcsWriter writer)
        {
            Guard.IsNotNull(writer, "writer");

            var content = new ContentLine { Name = "REQUEST-STATUS" };
            if (Description.Language != LanguageTag.Unspecified && Description.Language != null)
                content.Parameters[ParameterName.Language] = Description.Language.Name;
            var v = new StringBuilder();
            v.Append(Code ?? "");
            v.Append(';');
            v.Append(Description.Value);
            v.Append(';');
            v.Append(RelatedData ?? "");

            content.Values = new [] { v.ToString().TrimEnd(';') };
            writer.Write(content);
        }
    }
}
