using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Represents a strongly typed <see cref="Tag"/>.
    /// </summary>
    /// <remarks>
    ///   A strongly typed <see cref="Tag"/> is typically used to define some known <see cref="Tag.Name">names</see> as 
    ///   the following example demonstrates.  The <see cref="Unknown"/> definition is always defined.
    /// </remarks>
    /// <example>
    ///   A Gender <see cref="Tag{T}"/> could be defined as follows.
    /// 
    ///   <code source="SepiaExamples\TypedTagExample.cs" region="Gender" language="C#" />
    /// </example>
    public class Tag<T> : Tag, ITag<T>
        where T : ITag, new()
    {
        /// <summary>
        ///   Indicates the lack of any information.
        /// </summary>
        public static new T Unknown = new T
        {
            Authority = Tag.Unknown.Authority,
            Name = Tag.Unknown.Name,
            Description = Tag.Unknown.Description
        };

    }
}
