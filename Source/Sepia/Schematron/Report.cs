using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Sepia.Schematron
{
   /// <summary>
   ///   An assertion made about the context nodes.
   /// </summary>
   /// <remarks>
   ///   <b>Report</b> is an inverse <see cref="Assertion"/> made about the <see cref="Rule"/> context node(s).  If <see cref="Assertion.Test"/> 
   ///   evaluates to positive, then the <b>Report</b> succeeds and a <see cref="SchematronValidationEventArgs">validation event</see> is raised.
   /// </remarks>
   [Serializable]
   public sealed class Report : Assertion
   {
      /// <summary>
      ///   Converts a <see cref="Report"/> to an <see cref="Assertion"/>.
      /// </summary>
      /// <param name="negatedTest">
      ///   The negated <see cref="Assertion.Test"/> for the <see cref="Assertion"/>.
      /// </param>
      /// <returns>
      ///   A new <see cref="Assertion"/> that is equivalent to this <see cref="Report"/>. 
      /// </returns>
      /// <remarks>
      ///   <b>ToAssertion</b> is used to convert a <see cref="Report"/> to an <see cref="Assertion"/>.  The <paramref name="negatedTest"/>
      ///   must be negatation of the report's <see cref="Assertion.Test"/>.  For the XPath query language, the <c>not()</c> function can
      ///   be used.
      /// </remarks>
      public Assertion ToAssertion(string negatedTest)
      {
         Assertion a = new Assertion();
         a.Diagnostics = this.Diagnostics;
         a.Fpi = this.Fpi;
         a.Icon = this.Icon;
         a.ID = this.ID;
         a.Message = this.Message;
         a.Role = this.Role;
         a.See = this.See;
         a.Subject = this.Subject;
         a.Test = negatedTest;

         return a;
      }
   }
}
