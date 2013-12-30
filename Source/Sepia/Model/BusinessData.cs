using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Model
{
    /// <summary>
    ///   A <see cref="Resource"/> that can be used and/or transformed by a business process.
    /// </summary>
    public class BusinessData : Resource
    {
        const string EnterpriseName = "sepia"; // TODO: Configuration pattern.

        string id;
        string uri;

        /// <summary>
        ///   Creates a new instance of the <see cref="BusinessData"/> class.
        /// </summary>
        public BusinessData()
        {
            Tags = new TagBag();
        }

        /// <summary>
        ///   The unique identifier of the data within the enterprise.
        /// </summary>
        /// <remarks>
        ///   This is typically a <see cref="Guid"/> or a monotonically increasing number.
        /// </remarks>
        /// <exception cref="InvalidOperationException">When setting the <see cref="Id"/> and it already has a value.</exception>
        public string Id
        {
            get { return id; }
            set
            {
                Guard.IsMutable(id, "ID");
                id = value;
                uri = string.Format("urn:{0}:{1}:{2}", EnterpriseName, GetType().Name.ToLowerInvariant(), Id);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">When the <see cref="Id"/> has no value.</exception>
        public override string Uri
        {
            get
            {
                Guard.IsNotNull(uri, "BusinessData.Id");
                return uri;
            }
        }

        /// <summary>
        ///   Some classifications for the business data.
        /// </summary>
        /// <remarks>
        ///   Most properties of business data can be represented as tags.
        /// </remarks>
        public TagBag Tags { get; set; } 
    }
}
