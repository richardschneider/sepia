using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   A tag is used to classify another object.  
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   A tag consists of a short <see cref="Name"/> issued by some <see cref="Authority"/> and
    ///   a <see cref="Description"/>.  Its <see cref="Uri"/> has the form "urn:<see cref="Authority"/>:<see cref="Name"/>".
    ///  </para>
    /// </remarks>
    /// <example>
    ///   <code source="SepiaExamples\TagExample.cs" region="Creating" language="C#" />
    /// </example>
    public class Tag : Resource, ITag
    {
        /// <summary>
        ///   Indicates the lack of any information.
        /// </summary>
        public static Tag Unknown = new Tag { Authority = "sepia", Name = "Unknown", Description = { new Text("en", "Unknown") } };

        /// <summary>
        ///   Creates a new instance of the <see cref="Tag"/> class.
        /// </summary>
        public Tag()
        {
            Description = new MultilingualText();
        }

        /// <summary>
        ///   A short name for the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   The authority that issued the tag.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        ///   A multilingual description of the tag.
        /// </summary>
        public MultilingualText Description { get; set; }

        /// <summary>
        ///   The URI (Uniform Resource Identifier) representation of the tag. 
        /// </summary>
        /// <returns>
        ///   The URI representation of the tag in the form "urn:<see cref="Authority"/>:<see cref="Name"/>".
        /// </returns>
        public override string Uri
        {
            get { return string.Format("urn:{0}:{1}", Authority, Name); }
        }

        /// <summary>
        ///   Make a URN (Uniform Resource Name) from the supplied components.
        /// </summary>
        /// <param name="name">A short name for the tag.</param>
        /// <param name="authority">The authority that issued the tag.</param>
        /// <returns></returns>
        protected static string MakeUrn(string authority, string name)
        {
            return string.Format("urn:{0}:{1}", authority, name);
        }

    }
}
