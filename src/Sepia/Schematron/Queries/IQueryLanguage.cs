using Sepia.Schematron.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace Sepia.Schematron.Queries
{
   /// <summary>
   ///   A Schematron Query Language.
   /// </summary>
   /// <remarks>
   ///   A Schematron Query Language provides the implementation for the query expressions used in a <see cref="Rule"/> and 
   ///   an <see cref="Assertion"/>.
   /// </remarks>
   public interface IQueryLanguage : IProvider
   {
      /// <summary>
      ///   Creates a context for matching a <see cref="Rule"/> using the specified <see cref="SchematronDocument">Schematron schema</see>
      ///   and XML document.
      /// </summary>
      /// <param name="schematronSchema">
      ///   The <see cref="SchematronDocument"/> that supplies the rules. 
      /// </param>
      /// <param name="instance">
      ///   The <see cref="IXPathNavigable">XML document</see> to <see cref="Match"/>.
      /// </param>
      /// <returns>
      ///   A query language specific context.
      /// </returns>
       object CreateMatchContext(SchematronDocument schematronSchema, IXPathNavigable instance);

      /// <summary>
      ///   Assigns a value to a variable.
      /// </summary>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <param name="name">The name of the variable.</param>
      /// <param name="value">The value of the variable.  The value is interpreted as an expression.</param>
      /// <remarks>
      ///   Derived classes should <c>throw</c> if the variable has already been assigned a value in the current scope.
      /// </remarks>
      /// <seealso cref="PushScope"/>
      /// <seealso cref="PopScope"/>
      void Let(object context, string name, string value);

      /// <summary>
      ///   Determines whether the <see cref="Rule"/> matches the current node.
      /// </summary>
      /// <param name="rule">The <see cref="Rule"/> to match.</param>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <param name="instance">An <see cref="XPathNavigator"/> containing the current node.</param>
      /// <returns>
      ///   <b>true</b> if the <paramref name="rule"/> matches the <paramref name="instance"/> node.
      /// </returns>
      bool Match(Rule rule, object context, XPathNavigator instance);

      /// <summary>
      ///   Determines whether the <see cref="Assertion"/> is true.
      /// </summary>
      /// <param name="assertion">The <see cref="Assertion"/> to <see cref="Assertion.Test"/>.</param>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <param name="instance">An <see cref="XPathNavigator"/> containing the current node.</param>
      /// <returns>
      ///   <b>true</b> if the <paramref name="assertion"/> tests true.
      /// </returns>
      bool Assert(Assertion assertion, object context, XPathNavigator instance);

      /// <summary>
      ///   Pushes the variable scope.
      /// </summary>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      void PushScope(object context);

      /// <summary>
      ///   Pops the variable scope.
      /// </summary>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <remarks>
      ///   Its is an error to Pop if a scope has not been pushed.
      /// </remarks>
      void PopScope(object context);
   }
}
