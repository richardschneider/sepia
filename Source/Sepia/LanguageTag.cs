using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   An <see href="http://en.wikipedia.org/wiki/IETF_language_tag">IETF Language Tag</see> used to specify a language.
    /// </summary>
    /// <remarks>
    ///   A language tag is created by using <see cref="Resolve"/> so that a cache of tags can be used.  
    ///  <para>
    ///   A string can used whenever a <see cref="LanguageTag"/> is required (implicit casting).  It quietly calls <see cref="Resolve"/>.
    ///  </para>
    /// </remarks>
    public class LanguageTag : Tag
    {
        internal static IObjectCache Cache = null;
        const string Ietf = "ietf:bcp47";

        /// <summary>
        ///   The English (en) language tag.
        /// </summary>
        public static readonly LanguageTag English = LanguageTag.Resolve("en");

        /// <summary>
        ///   Used when the language is not specified. TODO: does IETF have a tag for this?
        /// </summary>
        public static readonly LanguageTag Unspecified = new LanguageTag
        {
            Authority = "sepia",
            Name = "x-unspecified",
            Description = { new Text(English, "not specified") }
        };

        /// <summary>
        ///   Explicitly creating a LanguageTag is not allowed because it would not be cached.  Use the <see cref="Resolve"/> method.
        /// </summary>
        private LanguageTag()
        {
        }

        /// <summary>
        ///   Implicit casting from <see cref="string"/>.
        /// </summary>
        /// <param name="tag">
        ///   A IETF language tag represented as a <see cref="string"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="LanguageTag"/> representation of the string.
        /// </returns>
        public static implicit operator LanguageTag(string tag)
        {
            return Resolve(tag);
        }

        /// <summary>
        ///   Find the <see cref="LanguageTag"/> with the specified name.
        /// </summary>
        /// <param name="name">The IETF language tag.</param>
        /// <returns>A <seealso cref="LanguageTag"/> for the <paramref name="name"/>.</returns>
        /// <remarks>
        ///   The <see cref="Tag.Description"/> will contain the English and native names of the
        ///   languages if they are available from <see cref="CultureInfo.GetCultureInfo(string)"/>.
        ///  <para>
        ///   Language tags are <see cref="IObjectCache">cached</see> to conserve memory usage.
        ///  </para>
        ///  <para>
        ///   If <paramref name="name"/> is null, then the <see cref="Unspecified"/> language tag is
        ///   returned.
        ///  </para>
        /// </remarks>
        public static LanguageTag Resolve(string name)
        {
            if (name == null)
                return Unspecified;

            Guard.IsNotNullOrWhiteSpace(name, "name");

            Func<string, LanguageTag> resolver = (key) => new LanguageTag { Name = name, Authority = Ietf };

            var tag = Cache == null
                ? resolver("")
                : Cache.Resolve(MakeUrn(Ietf, name), resolver);

            // Get a description for the tag. If we encounter an error, just ignore it.
            try
            {
                var culture = CultureInfo.GetCultureInfo(name);
                tag.Description.Add(new Text(tag, culture.NativeName));
                tag.Description.Add(new Text(English, culture.EnglishName));
            }
            catch (Exception)
            {
                // TODO: Log a warning.
            }

            return tag;
        }
    }
}
