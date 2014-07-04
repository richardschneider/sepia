using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Specifies that an object that implements the interface can be identified by an
    ///   URI (Uniform Resource Identifier).
    /// </summary>
    /// <remarks>
    ///   This is a base requirement for many objects in Sepia.  The <seealso cref="Resource"/> is the standard
    ///   implementation of this interface.
    /// <note>
    ///  URIs are case insensitive.
    /// </note>
    /// </remarks>
    /// <seealso cref="Resource"/>
    public interface IResolvable
    {
        /// <summary>
        ///   The Uniform Resource Identifier of the object. 
        /// </summary>
        /// <returns>
        ///   The <see cref="string"/> representation of a <see cref="System.Uri"/>.
        /// </returns>
        /// <remarks>
        ///   URIs are case insensitive and should never change once assigned.
        /// </remarks>
        string Uri { get; }
    }
}
