using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Allows invalidating and evicting a <see cref="IResolvable">resource</see> 
    ///   from a <see cref="IObjectCache">cache</see>.
    /// </summary>
    /// <example>
    ///   <code title="Canceling a credit card" source="SepiaExamples\ObjectCacheExample.cs" region="Deleting" language="C#" />
    /// </example>
    public static class ObjectCacheExtensions
    {
        /// <summary>
        ///   Signal that the <see cref="IResolvable"/> is no longer valid.
        /// </summary>
        /// <param name="cache">
        ///   The <see cref="IObjectCache"/> to change.
        /// </param>
        /// <param name="resource">
        ///   The <see cref="IResolvable"/> to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   When <paramref name="cache"/> or <paramref name="resource"/> is <b>null</b>.
        /// </exception>
        /// <remarks>
        ///   Removes the <paramref name="resource"/> from the <paramref name="cache"/> and any associated backing stores. 
        ///   No <see cref="Exception"/> is <c>thrown</c> if the resource does not exist in the cache.
        /// </remarks>
        public static void Invalidate(this IObjectCache cache, IResolvable resource)
        {
            Guard.IsNotNull(cache, "cache");
            Guard.IsNotNull(resource, "resource");

            cache.Invalidate(resource.Uri);
        }

        /// <summary>
        ///   Signal that the <see cref="IResolvable"/> is no longer required in the cache.
        /// </summary>
        /// <param name="cache">
        ///   The <see cref="IObjectCache"/> to change.
        /// </param>
        /// <param name="resource">
        ///   The <see cref="IResolvable"/> to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   When <paramref name="cache"/> or <paramref name="resource"/> is <b>null</b>.
        /// </exception>
        /// <remarks>
        ///   Removes the <paramref name="resource"/> from the <paramref name="cache"/>. This may place the resource into an associated backing store, 
        ///   based on the policies of the cache.  No <see cref="Exception"/> is <c>thrown</c> if
        ///   the resource does not exist in the store.
        /// </remarks>
        public static void Evict(this IObjectCache cache, IResolvable resource)
        {
            Guard.IsNotNull(cache, "cache");
            Guard.IsNotNull(resource, "resource");

            cache.Evict(resource.Uri);
        }

    }
}
