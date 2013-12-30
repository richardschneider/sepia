using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Provides an abstract base class for an object that is identifiable by its <see cref="Uri"/> and 
    ///   can be obtained (fetched) with a resolver.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   A resource is uniquely identified by its <see cref="Uri">Uniform Resource Identifier</see>, 
    ///   which is also its <see cref="ToString"/> result.
    ///  </para>
    ///  <para>
    ///   Equality and hashing are based on the resources's <see cref="Uri"/>, which is case insensitive.
    ///  </para>
    /// </remarks>
    public abstract class Resource : IResolvable, IEquatable<IResolvable>
    {
        /// <summary>
        ///   The URI (Uniform Resource Identifier) assigned to the resource. 
        /// </summary>
        /// <inheritdoc />
        public abstract string Uri { get; }

        /// <summary>
        ///   A human readable representation of the resource.
        /// </summary>
        /// <returns>
        ///   The resource's <see cref="Uri"/>.
        /// </returns>
        public override string ToString()
        {
            return Uri;
        }

        /// <summary>
        ///   Returns the hash code for the resource.
        /// </summary>
        /// <returns>
        ///   An <see cref="int"/> hash of the resource's <see cref="Uri"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Uri.ToLowerInvariant().GetHashCode();
        }

        /// <summary>
        ///   Determines if this and that are equal.
        /// </summary>
        /// <param name="that">
        ///   The other <see cref="Resource"/> to compare.
        /// </param>
        /// <returns>
        ///   <b>true</b> if both <see cref="Uri">URIs</see> are equal; otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   For two <see cref="Resource"/> objects to be considered equal both the <see cref="Uri">URIs</see>
        ///   must match in a case insensitive manner.
        /// </remarks>
        public bool Equals(IResolvable that)
        {
            return that != null
                && string.Equals(this.Uri, that.Uri, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///   Determines if this and that are equal.
        /// </summary>
        /// <param name="that">
        ///   The other <see cref="object"/> to compare.
        /// </param>
        /// <returns>
        ///   <b>true</b> if <paramref name="that"/> is a <see cref="Resource"/> and both are equal; 
        ///   otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   For two <see cref="Resource"/> objects to be considered equal both the <see cref="Uri">URIs</see>
        ///   must match in a case insensitive manner.
        /// </remarks>
        public override bool Equals(object that)
        {
            return that is Resource && this.Equals((IResolvable)that);
        }

        /// <summary>
        ///   Determines if a specified <see cref="Resource"/> is equal to another 
        ///   <see cref="Resource"/>.
        /// </summary>
        /// <param name="a">A <see cref="Resource"/>.</param>
        /// <param name="b">A <see cref="Resource"/>.</param>
        /// <returns>
        ///   <b>true</b> if <paramref name="a"/> is equal to <paramref name="b"/>; 
        ///   otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   For two <see cref="Resource"/> objects to be considered equal both the <see cref="Uri">URIs</see>
        ///   must match in a case insensitive manner.
        /// </remarks>
        /// <seealso cref="Equals(IResolvable)"/>
        public static bool operator ==(Resource a, Resource b)
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null))
                return false;
            if (object.ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        /// <summary>
        ///   Determines if a specified <see cref="Resource"/> is not equal to another 
        ///   <see cref="Resource"/>.
        /// </summary>
        /// <param name="a">A <see cref="Resource"/>.</param>
        /// <param name="b">A <see cref="Resource"/>.</param>
        /// <returns>
        ///   <b>true</b> if <paramref name="a"/> is not equal to <paramref name="b"/>; 
        ///   otherwise, <b>false</b>.
        /// </returns>
        public static bool operator !=(Resource a, Resource b)
        {
            return !(a == b);
        }

    }
}
