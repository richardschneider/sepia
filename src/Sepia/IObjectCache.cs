using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   A cache for an object.
    /// </summary>
    /// <remarks>
    ///   All objects use an URI to uniquely identify itself.  The cache must be thread safe.
    /// <note type="implement">
    ///   A URI is case insensitive.
    /// </note>
    /// </remarks>
    /// <example>
    ///   Get an image from the cache or from the web.
    /// 
    ///   <code title="Resolving a URI" source="SepiaExamples\ObjectCacheExample.cs" region="Resolving" language="C#" />
    /// </example>
    public interface IObjectCache
    {
        /// <summary>
        ///   Get an instance of the object identified by <paramref name="uri"/>.
        /// </summary>
        /// <typeparam name="T">
        ///   The <see cref="Type"/> of object returned by the <paramref name="loader"/>.
        /// </typeparam>
        /// <param name="uri">
        ///   A globally unique name for the object.
        /// </param>
        /// <param name="loader">
        ///   A function to load the object when it is not cached.
        /// </param>
        /// <returns>
        ///   The object identified by <paramref name="uri"/>
        /// </returns>
        /// <exception cref="CircularDependencyException">
        ///   A uri this is being resolved also depends on a uri that is being resolved.
        /// </exception>
        /// <remarks>
        ///   If the object identified by the <paramref name="uri"/> is in any of the cache(s) then return it.
        ///   Otherwise, use the <paramref name="loader"/> function to instantiate the object and then place it
        ///   in the cache.
        /// <note type="implement">
        ///   The <paramref name="loader"/> may have circular references which <b>MUST</b> be detected and a <see cref="CircularDependencyException"/>
        ///   thrown.  It is good practice to include the resolving order of the URIs in the <see cref="Exception.Message"/>,
        /// </note>
        /// </remarks>
        /// <example>
        ///   Get an image from the cache or from the web.
        /// 
        ///   <code title="Resolving a URI" source="SepiaExamples\ObjectCacheExample.cs" region="Resolving" language="C#" />
        /// </example>
        T Resolve<T>(string uri, Func<string, T> loader);

        /// <summary>
        ///   Signal that the object identified by <paramref name="uri"/> is no longer valid.
        /// </summary>
        /// <param name="uri">
        ///   A unique name for the object.
        /// </param>
        /// <remarks>
        ///   Removes the object from the cache and any associated backing stores. No <see cref="Exception"/> is <b>thrown</b> if
        ///   the object does not exist in the store.
        /// </remarks>
        void Invalidate(string uri);

        /// <summary>
        ///   Signal that the object identified by <paramref name="uri"/> is no longer required.
        /// </summary>
        /// <param name="uri">
        ///   A unique name for the object.
        /// </param>
        /// <remarks>
        ///   Removes the object from this cache. This may place the object into an associated backing store, based on the
        ///   policies of the cache.  No <see cref="Exception"/> is <b>thrown</b> if
        ///   the object does not exist in the store.
        /// </remarks>
        void Evict(string uri);
    }
}
