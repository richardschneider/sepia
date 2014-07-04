using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{

    /// <summary>
    ///   A collection of <see cref="Text"/> that represents the same concept in multiple languages.
    /// </summary>
    /// <example>
    ///   <code title="MultilingualText Hello World" source="SepiaExamples\MultilingualTextExample.cs" region="Hello World" language="C#" />
    /// </example>
    public class MultilingualText : List<Text>
    {
        /// <summary>
        ///   Gets the <see cref="Text"/> that is best is suited for the specified <see cref="LanguageTag"/> from the list.
        /// </summary>
        /// <param name="language">
        ///   The <see cref="LanguageTag"/> of the <see cref="Text"/> that is needed. A <see cref="string"/> 
        ///   can also be used because it implicitly casts to a <see cref="LanguageTag"/>; such as "zh-Hant" or "zh-TW".
        /// </param>
        /// <returns>
        ///   Some appropriate text for the specified <paramref name="language"/>.  If the list is empty,
        ///   then <see cref="Text.Empty"/> is returned.
        /// </returns>
        /// <remarks>
        ///   This is sugar for the <see cref="TextExtensions.WrittenIn"/> method.  If <see cref="Text"/>
        ///   for the specified <paramref name="language"/> is not found, then  a best guess will be returned.
        ///   When the list is empty, <see cref="Text.Empty"/> is returned.
        /// </remarks>
        /// <example>
        ///   <code title="MultilingualText Hello World" source="SepiaExamples\MultilingualTextExample.cs" region="Hello World" language="C#" />
        /// </example>
        public Text this[LanguageTag language]
        {
            get { return this.WrittenIn(language); }
        }

    }
}
