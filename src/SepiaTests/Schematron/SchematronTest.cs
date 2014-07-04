using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sepia.Schematron.Tests
{
   public class SchematronTest
   {
      public static SchematronDocument Load(string uri)
      {
         return SchematronReader.ReadSchematron(uri);
      }

      public Rule FindRule(SchematronDocument a, string id)
      {
         foreach (Pattern p in a.Patterns)
         {
            foreach (Rule r in p.Rules)
            {
               if (r.ID == id)
                  return r;
            }
         }
         throw new Exception(String.Format("'{0}' is not a rule.", id));
      }

      public bool TryValidating(SchematronValidator validator, XmlDocument instance)
      {
         try
         {
            validator.Validate(instance);
            return true;
         }
         catch (SchematronValidationException)
         {
            return false;
         }
      }
   }
}
