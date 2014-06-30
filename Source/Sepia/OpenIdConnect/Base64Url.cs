using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   A Base64 URL codec.
    /// </summary>
    /// <remarks>
    ///  Base64 encoding using the URL- and filename-safe character set
    ///  defined in Section 5 of RFC 4648 [RFC4648], with all trailing '='
    ///  characters omitted (as permitted by Section 3.2) and without the
    ///  inclusion of any line breaks, white space, or other additional
    ///  characters.  (See Appendix C for notes on implementing base64url
    ///  encoding without padding.)
    /// </remarks>
    public static class Base64Url
    {
        const char Base64PadCharacter = '=';
        const char Base64Character62 = '+';
        const char Base64Character63 = '/';
        const char Base64UrlCharacter62 = '-';
        const char Base64UrlCharacter63 = '_';

        /// <summary>
        ///   Decodes the Base64 URL encoded string.
        /// </summary>
        public static byte[] Decode(string value)
        {
            Guard.IsNotNullOrWhiteSpace(value, "value");

            StringBuilder s = new StringBuilder(value);
            s.Replace(Base64UrlCharacter62, Base64Character62);
            s.Replace(Base64UrlCharacter63, Base64Character63);

            int pad = s.Length % 4;
            s.Append(Base64PadCharacter, (pad == 0) ? 0 : 4 - pad);

            return Convert.FromBase64String(s.ToString());
        }

    }
}
