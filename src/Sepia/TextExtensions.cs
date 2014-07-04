using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Extensions methods for <see cref="Text"/>.
    /// </summary>
    public static class TextExtensions
    {
        /// <summary>
        ///   Determines if the <see cref="IEnumerable{Text}"/> contains a value
        ///   for a specific language code.
        /// </summary>
        /// <param name="enumerable">
        ///   An <see cref="IEnumerable{Text}"/> to search.
        /// </param>
        /// <param name="language">
        ///   The IETF language code to find.
        /// </param>
        /// <returns>
        ///   <b>true</b> if a <see cref="Text"/> exists with the exact <paramref name="language"/>.
        /// </returns>
        public static bool HasExactValue(this IEnumerable<Text> enumerable, LanguageTag language)
        {
            return enumerable.Any(t => t.Language == language);
        }

        /// <summary>
        ///   Find some <see cref="Text"/> that is appropriate for the specified language tag.
        /// </summary>
        /// <param name="enumerable">
        ///   An enumerable list of <see cref="Text"/>.
        /// </param>
        /// <param name="language">
        ///   The <see cref="LanguageTag"/> of the <see cref="Text"/> that is needed. A <see cref="string"/>,
        ///   such as "zh-Hant" or "zh-TW", can also be used because it implicitly casts to a <see cref="LanguageTag"/>.
        /// </param>
        /// <returns>
        ///   Some appropriate text from the <paramref name="enumerable"/> for the specified <paramref name="language"/>.
        ///   If <paramref name="enumerable"/> is empty, then <see cref="Text.Empty"/> is returned.
        /// </returns>,
        /// <remarks>
        ///   TODO: 
        /// </remarks>
        /// <example>
        ///   <code title="Multilingual Hello World" source="SepiaExamples\TextExample.cs" region="Hello World" language="C#" />
        /// </example>
        public static Text WrittenIn(this IEnumerable<Text> enumerable, LanguageTag language)
        {
            Guard.IsNotNull(language, "language");

            var subtag = language.Name.Trim().ToLowerInvariant();
            while (subtag != "")
            {
                foreach (var t in enumerable)
                {
                    if (string.Equals(t.Language.Name, subtag, StringComparison.InvariantCultureIgnoreCase))
                        return t;
                }

                int dash = subtag.LastIndexOf('-');
                if (dash < 0)
                    break;
                subtag = subtag.Remove(dash);
            }

            // TODO: Find the script of language required and then return something close.

            // Final fall-back, return the first element.
            return enumerable.DefaultIfEmpty(Text.Empty).First();
        }

    }
}
