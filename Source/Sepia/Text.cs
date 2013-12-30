using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    /// <summary>
    ///   Some word(s) in a specific language.
    /// </summary>
    /// <remarks>
    ///   An <see href="http://en.wikipedia.org/wiki/IETF_language_tag">IETF Language Tag</see> is used to specify the <see cref="Language"/>
    ///   of the <see cref="Value"/>.
    ///  <para>
    ///   <b>Text</b> is immutable and uses value equality.  It is also implicitly cast to a <see cref="string"/>.
    ///  </para>
    ///  <para>
    ///   <see cref="TextExtensions"/> exists to manipulate enumerable collections of Text.
    /// </para>
    /// </remarks>
    /// <example>
    ///   <code title="Multilingual Hello World" source="SepiaExamples\TextExample.cs" region="Hello World" language="C#" />
    /// </example>
    /// <seealso cref="LanguageTag"/>
    /// <seealso cref="TimeExtensions"/>
    public struct Text : IEquatable<Text>
    {
        /// <summary>
        ///   The <see cref="Text"/> equivalent of <see cref="string.Empty"/>.
        /// </summary>
        /// <remarks>
        ///   The <see cref="Language"/> is <see cref="LanguageTag.Unspecified"/> and the value is <see cref="string.Empty"/>
        /// </remarks>
        public static readonly Text Empty = new Text(LanguageTag.Resolve("x-unspecified"), string.Empty);

        /// <summary>
        ///   Represents the default (uninitialized) value of a <see cref="Text"/>.
        /// </summary>
        /// <remarks>
        ///   Both <see cref="Language"/> and <see cref="Value"/> are <b>null</b>.
        /// </remarks>
        public static readonly Text Default = new Text();

        LanguageTag language;
        string value;

        /// <summary>
        ///   Creates a instance of the <see cref="Text"/> class with the specified <see cref="Language"/>
        ///   and <see cref="Value"/>.
        /// </summary>
        /// <param name="language">
        ///   The <see href="http://en.wikipedia.org/wiki/IETF_language_tag">IETF language tag</see> for the Text.
        /// </param>
        /// <param name="s">
        ///   The <see cref="string"/> value for the Text.
        /// </param>
        public Text(LanguageTag language, string s)
        {
            Guard.IsNotNull(language, "language");
            Guard.IsNotNull(s, "s");

            this.language = language;
            this.value = s;
        }
         
        /// <summary>
        ///   IETF language tag that represents a language and possible culture.
        /// </summary>
        /// <remarks>
        ///   Languages tags are case insensitive and <see cref="string.Intern">interned</see> because there are usually just
        ///   a handful of tags in use.
        ///  <para>
        ///   To compare two language tags, use the snippet <c>string.Equals(aTag, otherTag, StringComparison.InvariantCultureIgnoreCase)</c>
        ///  </para>
        /// </remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/IETF_language_tag">Wikipedia</seealso>.
        public LanguageTag Language
        {
            get { return language; }
            set
            {
                Guard.IsMutable(language, "Text.Langauge");
                language = value;
            }
        }

        /// <summary>
        ///   The string value in the <see cref="Language"/>.
        /// </summary>
        public string Value
        {
            get { return value; }
            set
            {
                Guard.IsMutable(this.value, "Text.Value");
                this.value = value;
            }
        }

        /// <summary>
        ///   The string representation of the <see cref="Text"/>
        /// </summary>
        /// <returns>
        ///   A string in the form "<i><see cref="Value"/></i> (<i>Language.Name</i>)".
        /// </returns>
        public override string ToString()
        {
            if (Language == null)
                return Value;

            return string.Format("{0} ({1})", Value, Language.Name);
        }

        /// <summary>
        ///   Returns the hash code for the <see cref="Text"/>.
        /// </summary>
        /// <returns>
        ///   An <see cref="int"/> hash of the <see cref="Language"/> and <see cref="Value"/>.
        /// </returns>
        public override int GetHashCode()
        {
            if (Language == null || Value == null)
                throw new InvalidOperationException("Both Text.Language and Text.Value must be specified before a hash code can be obtained.");

            return Language.GetHashCode() ^ Value.GetHashCode();
        }

        /// <summary>
        ///   Determines if this and that are equal.
        /// </summary>
        /// <param name="that">
        ///   The other <see cref="Text"/> to compare.
        /// </param>
        /// <returns>
        ///   <b>true</b> if both <see cref="Language"/> and <see cref="Value"/> are equal; otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   For two <see cref="Text"/> objects to be considered equal both the <see cref="Language"/>
        ///   and <see cref="Value"/> must match.  <see cref="Language"/> is case insensitive.
        /// </remarks>
        public bool Equals(Text that)
        {
            return this.Language == that.Language && this.Value == that.Value;
        }

        /// <summary>
        ///   Determines if this and that are equal.
        /// </summary>
        /// <param name="that">
        ///   The other <see cref="object"/> to compare.
        /// </param>
        /// <returns>
        ///   <b>true</b> if <paramref name="that"/> is a <see cref="Text"/> and both are equal; 
        ///   otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   For two <see cref="Text"/> objects to be considered equal both the <see cref="Language"/>
        ///   and <see cref="Value"/> must match.  <see cref="Language"/> is case insensitive.
        /// </remarks>
        public override bool Equals(object that)
        {
            return that is Text && this.Equals((Text)that);
        }

        /// <summary>
        ///   Determines if a specified <see cref="Text"/> is equal to another 
        ///   <see cref="Text"/>.
        /// </summary>
        /// <param name="a">A <see cref="Text"/>.</param>
        /// <param name="b">A <see cref="Text"/>.</param>
        /// <returns>
        ///   <b>true</b> if <paramref name="a"/> is equal to <paramref name="b"/>; 
        ///   otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   For two <see cref="Text"/> objects to be considered equal both the <see cref="Language"/>
        ///   and <see cref="Value"/> must match.  <see cref="Language"/> is case insensitive.
        /// </remarks>
        /// <seealso cref="Equals(Text)"/>
        public static bool operator ==(Text a, Text b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///   Determines if a specified <see cref="Text"/> is not equal to another 
        ///   <see cref="Text"/>.
        /// </summary>
        /// <param name="a">A <see cref="Text"/>.</param>
        /// <param name="b">A <see cref="Text"/>.</param>
        /// <returns>
        ///   <b>true</b> if <paramref name="a"/> is not equal to <paramref name="b"/>; 
        ///   otherwise, <b>false</b>.
        /// </returns>
        public static bool operator !=(Text a, Text b)
        {
            return !(a == b);
        }

        /// <summary>
        ///   Implicit casting to a <see cref="string"/>.
        /// </summary>
        /// <param name="text">
        ///   The <see cref="Text"/> to return as a <see cref="string"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="Value"/> of the <paramref name="text"/>.  If <paramref name="text"/> is <b>null</b>, 
        ///   then <see cref="string.Empty"/> is returned.
        /// </returns>
        public static implicit operator string(Text text)
        {
            return text.Value;
        }

    }
}
