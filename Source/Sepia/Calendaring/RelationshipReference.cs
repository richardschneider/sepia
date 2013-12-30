using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Represent a relationship or reference between one calendar component and another.
    /// </summary>
    public class RelationshipReference : IcsSerializable
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="RequestStatus"/> class.
        /// </summary>
        public RelationshipReference()
        {
            Relationship = Relationship.Parent;
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="RelationshipReference"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the request status.
        /// </param>
        public RelationshipReference(ContentLine content) : this()
        {
            Guard.IsNotNull(content, "content");
            Guard.Check(content.Name.Equals("RELATED-TO", StringComparison.InvariantCultureIgnoreCase), "content", "Expected a RELATED-TO content line.");

            OtherUri = content.Value;
            if (content.HasParameters)
            {
                var name = content.Parameters["RELTYPE"];
                if (name != null)
                    Relationship = new Relationship { Name = name };
            }
        }

        /// <summary>
        ///   The URI of the related calendar component.
        /// </summary>
        public string OtherUri { get; set; }

        /// <summary>
        ///   Identifies the relationship to the <see cref="OtherUri"/>.
        /// </summary>
        /// <value>
        ///   The default value is <see cref="Sepia.Calendaring.Relationship.Parent"/>.
        /// </value>
        public Relationship Relationship { get; set; }

        /// <inheritdoc />
        public void ReadIcs(IcsReader reader) // TODO
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void WriteIcs(IcsWriter writer)
        {
            var content = new ContentLine { Name = "related-to", Value = OtherUri };
            if (this.Relationship != null)
                content.Parameters[ParameterName.RelationshipType] = Relationship.Name;
            writer.Write(content);
        }
    }
}
