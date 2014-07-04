using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    class TypedTagExample
    {
        #region Gender
        public class Gender : Tag<Gender>
        {
            public static readonly Gender Male = new Gender { Authority = "Me", Name = "M" };
            public static readonly Gender Female = new Gender { Authority = "Me", Name = "F" };
        }
        #endregion
    }
}
