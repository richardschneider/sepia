using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   A collection of <see cref="ITag">tags</see>.
    /// </summary>
    public class TagBag : KeyedCollection<string, ITag>
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="TagBag"/> class.
        /// </summary>
        public TagBag() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <inheritdoc/>
        protected override string GetKeyForItem(ITag item)
        {
            return item.Uri;
        }

        /// <summary>
        ///   Adds the enumerable tags to the bag.
        /// </summary>
        /// <param name="tags"></param>
        public void AddRange(IEnumerable<ITag> tags)
        {
            foreach (var tag in tags)
            {
                Add(tag);
            }
        }
    }
}
