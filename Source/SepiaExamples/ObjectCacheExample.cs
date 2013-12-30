using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    using System.Drawing;
    using System.Net;

    public class ObjectCacheExample
    {
        #region Deleting
        public class CreditCard : IResolvable
        {
            public IObjectCache appCache;

            public string Uri { get; set; }

            void OnCancel()
            {
                appCache.Invalidate(this);
            }
        }
        #endregion
        
        #region Resolving
        public IObjectCache cache;

        Image GetWebImage(string url)
        {
            return cache.Resolve(url, (uri) =>
            {
                using (var stream = new WebClient().OpenRead(uri))
                {
                    return Image.FromStream(stream);
                }
            });
        }

        #endregion

        void Snippets()
        {
        }
    }
}
