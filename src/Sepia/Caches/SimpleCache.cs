using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sepia.Caches
{
    using System.Collections.Specialized;

    /// <summary>
    ///   A cache that is thread safe but not performant.  Should only be used in testing scenarios.
    /// </summary>
    /// <inheritdoc />
    /// <threadsafety instance="true" />
    public class SimpleCache : IObjectCache
    {
        /// <summary>
        ///   Every access has to be locked!  Very non-performant.
        /// </summary>
        readonly Hashtable cache = new Hashtable();

        /// <summary>
        ///   A list of the uri that are currently being resolved.  Assumes that the cache is locked.
        /// </summary>
        readonly StringCollection inprogess = new StringCollection();

        /// <inheritdoc/>
        public T Resolve<T>(string uri, Func<string, T> loader)
        {
            Guard.IsNotNull(uri, "uri");
            uri = uri.ToLowerInvariant().Trim();

            lock (cache)
            {
                if (cache.ContainsKey(uri))
                    return (T) cache[uri];

                if (inprogess.Contains(uri))
                {
                    throw new CircularDependencyException(string.Format("Circular dependency detected on '{0}'. The resolving order was {1}.",
                        uri, string.Join(", ", inprogess.OfType<string>())));                 
                }
                try
                {
                    inprogess.Add(uri);
                    var result = loader(uri);
                    cache.Add(uri, result);
                    return result;
                }
                finally
                {
                    inprogess.Remove(uri);
                }
            }
        }

        /// <inheritdoc/>
        public void Invalidate(string uri)
        {
            Evict(uri);
        }

        /// <inheritdoc/>
        public void Evict(string uri)
        {
            Guard.IsNotNull(uri, "uri");
            uri = uri.ToLowerInvariant().Trim();

            lock (cache)
            {
                cache.Remove(uri);
            }
        }
    }
}
