using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    class TagExample
    {
        void Snippets()
        {
            #region Creating
            var cold = new Tag
            {
                Authority = "SNOMED",
                Name = "82272006",
                Description = { new Text("en", "Common cold (disorder)") }
            };
            #endregion

        }
    }
}
