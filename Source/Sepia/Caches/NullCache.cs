using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Caches
{
    /// <summary>
    ///   A cache that nevers caches anything.
    /// </summary>
    public class NullCache : IObjectCache
    {
        /// <inheritdoc/>
        public T Resolve<T>(string uri, Func<string, T> loader)
        {
            return loader(uri);
        }

        /// <inheritdoc/>
        public void Invalidate(string uri)
        {
        }

        /// <inheritdoc/>
        public void Evict(string uri)
        {
        }
    }
}
