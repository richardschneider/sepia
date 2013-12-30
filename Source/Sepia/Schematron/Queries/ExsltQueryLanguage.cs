using System.Xml.XPath;
using System.Xml.Xsl;

namespace Sepia.Schematron.Queries
{
   /// <summary>
   ///   A Schematron Query Language that binds to "exslt".
   /// </summary>
   /// <remarks>
   ///   Provides access to the EXSLT functions as implemented by MVP XML Library - EXSLT.NET Module.
   /// </remarks>
   public class ExsltQueryLanguage : XsltQueryLanguage
   {
      internal override QueryContext CreateContext()
      {
         return new ExsltContext();
      }

      internal class ExsltContext : XPathQueryLanguage.QueryContext
      {
         Mvp.Xml.Exslt.ExsltContext mvpContext;

         public ExsltContext()
         {
            mvpContext = new Mvp.Xml.Exslt.ExsltContext(base.NameTable);
         }

         public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
         {
            if (string.IsNullOrEmpty(prefix))
               return base.ResolveFunction(prefix, name, argTypes);

            return mvpContext.ResolveFunction(prefix, name, argTypes);
         }
      }
   }
}
