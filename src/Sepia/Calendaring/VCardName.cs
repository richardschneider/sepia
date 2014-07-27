using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   A Name vCard value.
    /// </summary>
    public class VCardName : VCardValue
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VCardName"/> class.
        /// </summary>
        public VCardName()
        {
            FamilyNames = new List<string>(0);
            GivenNames = new List<string>(0);
            AdditionalNames = new List<string>(0);
            Prefixes = new List<string>(0);
            Suffixes = new List<string>(0);
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="VCardName"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the property parameters.
        /// </param>
        public VCardName(ContentLine content)
            : base(content)
        {
            var parts = content.Value.Split(';');
            FamilyNames = parts.Length > 0 ? parts[0].Split(',').ToList() : new List<string>(0);
            GivenNames = parts.Length > 1 ? parts[1].Split(',').ToList() : new List<string>(0);
            AdditionalNames = parts.Length > 2 ? parts[2].Split(',').ToList() : new List<string>(0);
            Prefixes = parts.Length > 3 ? parts[3].Split(',').ToList() : new List<string>(0);
            Suffixes = parts.Length > 4 ? parts[4].Split(',').ToList() : new List<string>(0);
        }

        /// <summary>
        ///   The family name(s).
        /// </summary>
        public List<string> FamilyNames { get; set; }

        /// <summary>
        ///   The given name(s).
        /// </summary>
        public List<string> GivenNames { get; set; }

        /// <summary>
        ///   The additional name(s).
        /// </summary>
        public List<string> AdditionalNames { get; set; }

        /// <summary>
        ///   The honorific prefixes.
        /// </summary>
        public List<string> Prefixes { get; set; }

        /// <summary>
        ///   The honorific prefixes.
        /// </summary>
        public List<string> Suffixes { get; set; }

        /// <inheritdoc />
        public override ContentLine ToContentLine(ContentLine content = null)
        {
            content = base.ToContentLine(content);
            
            var s = new StringBuilder();
            var empty = new List<string>(0);
            s.Append(string.Join(",", (FamilyNames ?? empty).ToArray()));
            s.Append(';');
            s.Append(string.Join(",", (GivenNames ?? empty).ToArray()));
            s.Append(';');
            s.Append(string.Join(",", (AdditionalNames ?? empty).ToArray()));
            s.Append(';');
            s.Append(string.Join(",", (Prefixes ?? empty).ToArray()));
            s.Append(';');
            s.Append(string.Join(",", (Suffixes ?? empty).ToArray()));
            content.Values = s.ToString().Split(',');

            return content;
        }

    }
}
