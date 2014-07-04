using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Specifies that an object that implements the interface that can be be used to classify another object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   A tag is used to classify another object.  It consists of a short <see cref="Name"/> issued by some <see cref="Authority"/> and
    ///   a <see cref="Description"/>. Equality and hashing are based on the tag's <see cref="IResolvable.Uri">URI</see> which is case insensitive.
    ///  </para>
    ///  <para>
    ///   A strongly type tag can be defined with <see cref="Tag{T}"/>, whereas <see cref="Tag"/> provides a standard tag.
    ///  </para>
    /// </remarks>
    public interface ITag : IResolvable
    {
        /// <summary>
        ///   A short name for the tag.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///   The authority that issued the tag.
        /// </summary>
        string Authority { get; set; }

        /// <summary>
        ///   A multilingual description of the tag.
        /// </summary>
        MultilingualText Description { get; set; }
    }

    /// <summary>
    ///   Specifies that an object that implements the interface can be be used to classify another object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   A tag is used to classify another object.  It consists of a short <see cref="ITag.Name"/> issued by some <see cref="ITag.Authority"/> and
    ///   a <see cref="ITag.Description"/>. Equality and hashing are based on the tag's <see cref="IResolvable.Uri">URI</see> which is case insensitive.
    ///  </para>
    ///  <para>
    ///   <see cref="Tag{T}"/> is the standard implementation of this interface.
    ///  </para>
    /// </remarks>
    public interface ITag<T> : ITag
    {

    }

}
