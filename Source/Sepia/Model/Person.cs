using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Model
{
    /// <summary>
    ///   A self aware entity.
    /// </summary>
    public class Person : BusinessData
    {
        /// <summary>
        ///   The known names of a person.
        /// </summary>
        public MultilingualText Names { get; set; }

        /// <summary>
        ///   The sex of a <see cref="Person"/>
        /// </summary>
        public Gender Gender
        {
            get { return Tags.OfType<Gender>().DefaultIfEmpty(Gender.Unknown).First(); }
            // TODO: Need set
        }
    }

    #region Tags
    /// <summary>
    ///   The sex of a <see cref="Person"/>.
    /// </summary>
    public class Gender : Tag<Gender>
    {
        /// <summary>
        ///   TODO
        /// </summary>
        public static readonly Gender Male = new Gender { Name = "M" };
    }

    #endregion
}
