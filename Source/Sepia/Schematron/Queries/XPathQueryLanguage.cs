using Common.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Sepia.Schematron.Queries
{
   /// <summary>
   ///   A Schematron Query Language that binds to "xpath".
   /// </summary>
   /// <remarks>
   ///   <b>XPathQueryLanguage</b> supports the XML Path Language (XPath) Version 1.0 (W3C Recommendation 16 November 1999) at 
   ///   <see href="www.w3.org/TR/xpath">www.w3.org/TR/xpath</see>.
   /// </remarks>
   public class XPathQueryLanguage : IQueryLanguage // TODO: XSLT and XPath query language.  XSLT has more functions.
   {
      private static ILog log = LogManager.GetLogger(typeof(XPathQueryLanguage));

      private string name;
      private string description;
      private XPathExpression current = XPathExpression.Compile(".");

      #region IQueryLanguage Members

      /// <summary>
      ///   Creates a context for matching a <see cref="Rule"/> using the specified <see cref="SchematronDocument">Schematron schema</see>
      ///   and <see cref="XmlDocument"/>.
      /// </summary>
      /// <param name="schematronSchema">
      ///   The <see cref="SchematronDocument"/> that supplies the rules. 
      /// </param>
      /// <param name="instance">
      ///   The <see cref="XmlDocument"/> instance to <see cref="Match"/>.
      /// </param>
      /// <returns>
      ///   A query language specific context.
      /// </returns>
      public object CreateMatchContext(SchematronDocument schematronSchema, XmlDocument instance)
      {
         if (instance == null)
            throw new ArgumentNullException("instance");

         QueryContext context = CreateContext();

         if (schematronSchema != null && schematronSchema.HasNamespaces)
         {
            foreach (NamespaceDefinition ns in schematronSchema.Namespaces)
            {
               context.AddNamespace(ns.Prefix, ns.Uri);
            }
         }

         context.current = instance.CreateNavigator().Select(current);

         return context;
      }

      internal virtual QueryContext CreateContext()
      {
         return new QueryContext();
      }

      /// <summary>
      ///   Assigns a value to a variable.
      /// </summary>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <param name="name">The name of the variable.</param>
      /// <param name="value">The value (expression) of the variable.</param>
      /// <remarks>
      ///   Derived classes should <c>throw</c> if the variable has already been assigned a value in the current scope.
      /// </remarks>
      /// <seealso cref="PushScope"/>
      /// <seealso cref="PopScope"/>
      public void Let(object context, string name, string value)
      {
         if (context == null)
            throw new ArgumentNullException("context");
         if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("name");
         if (value == null)
            throw new ArgumentNullException("value");

         QueryContext qcontext = (QueryContext)context;
         qcontext.variables.Add(name, value);

         if (log.IsDebugEnabled)
            log.Debug(String.Format("Let {0} = {1}", name, value));
      }


      /// <summary>
      ///   Determines whether the <see cref="Rule"/> matches the current node.
      /// </summary>
      /// <param name="rule">The <see cref="Rule"/> to match.</param>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <param name="instance">An <see cref="XPathNavigator"/> containing the current node.</param>
      /// <returns>
      ///   <b>true</b> if the <paramref name="rule"/> matches the <paramref name="instance"/> node.
      /// </returns>
      public bool Match(Rule rule, object context, XPathNavigator instance)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         if (context == null)
            throw new ArgumentNullException("context");
         if (instance == null)
            throw new ArgumentNullException("instance");

         lock (rule)
         {
             QueryContext qcontext = (QueryContext)context;
             XPathExpression expression = (XPathExpression)rule.QueryExpression;

             // Compile the expression if not yet done.
             if (expression == null)
             {
                 // Microsoft XPath, not sure about Mono, does not like a variable expression on a match!
                 string ruleContext = rule.Context.Trim();
                 if (ruleContext.StartsWith("$"))
                 {
                     Variable variable = qcontext.variables.Find(ruleContext.Remove(0, 1));
                     if (variable == null)
                         throw new ArgumentException(String.Format("'{0}' is not defined.", ruleContext.Remove(0, 1)));
                     expression = XPathExpression.Compile(variable.value);

                     if (log.IsDebugEnabled)
                         log.Debug("Binding context " + ruleContext + " to " + variable.value);
                 }
                 else
                 {
                     if (log.IsDebugEnabled)
                         log.Debug("Compiling context " + ruleContext);

                     expression = XPathExpression.Compile(ruleContext);
                     rule.QueryExpression = expression;
                 }
             }

             // Do the match.
             expression.SetContext(qcontext);
             bool q = instance.Matches(expression);
             if (q)
                 qcontext.current = instance.Select(current);

             return q;
         }
      }

      /// <summary>
      ///   Determines whether the <see cref="Assertion"/> is true.
      /// </summary>
      /// <param name="assertion">The <see cref="Assertion"/> to <see cref="Assertion.Test"/>.</param>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <param name="instance">An <see cref="XPathNavigator"/> containing the current node.</param>
      /// <returns>
      ///   <b>true</b> if the <paramref name="assertion"/> tests true.
      /// </returns>
      public bool Assert(Assertion assertion, object context, XPathNavigator instance)
      {
         if (assertion == null)
            throw new ArgumentNullException("assertion");
         if (context == null)
            throw new ArgumentNullException("context");
         if (instance == null)
            throw new ArgumentNullException("instance");

         object result;
         bool ok = false;
         lock (assertion)
         {
             XPathExpression expression = assertion.QueryExpression as XPathExpression;
             if (expression == null)
             {
                 if (log.IsDebugEnabled)
                     log.Debug("Compiling test " + assertion.Test);

                 expression = XPathExpression.Compile(assertion.Test);
                 assertion.QueryExpression = expression;
             }
             expression.SetContext((QueryContext)context);
             result = instance.Evaluate(expression);
             switch (expression.ReturnType)
             {
                 case XPathResultType.Boolean:
                     ok = (bool)result;
                     break;
                 case XPathResultType.NodeSet:
                     ok = ((XPathNodeIterator)result).Count != 0;
                     break;
                 case XPathResultType.Number:
                     ok = ((double)result) != 0.0;
                     break;
                 case XPathResultType.String:
                     ok = XmlConvert.ToBoolean((string)result); // Check XPATH spec for compliance of casting string to bool
                     break;
                 default:
                     throw new NotSupportedException();
             }
         }

         bool isReport = assertion is Report;
         if (isReport && ok)
            return false;
         if (!isReport && !ok)
            return false;

         return true;
      }

      /// <summary>
      ///   Pushes the variable scope.
      /// </summary>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      public void PushScope(object context)
      {
         if (context == null)
            throw new ArgumentNullException("context");

         QueryContext qcontext = (QueryContext)context;
         qcontext.variables.PushScope();
      }

      /// <summary>
      ///   Pops the variable scope.
      /// </summary>
      /// <param name="context">The query language context, as returned from <see cref="CreateMatchContext"/>.</param>
      /// <remarks>
      ///   Its is an error to Pop if a scope has not been pushed.
      /// </remarks>
      public void PopScope(object context)
      {
         if (context == null)
            throw new ArgumentNullException("context");

         QueryContext qcontext = (QueryContext)context;
         qcontext.variables.PopScope();
      }
      
      #endregion

      #region IProvider Members

      /// <summary>
      ///   Initialises the provider with a collection of configuration settings.
      /// </summary>
      /// <param name="settings">
      ///   A <see cref="NameValueCollection"/> containing the configuration settings for the provider.  This can be <b>null</b>.
      /// </param>
      public void Initialize(NameValueCollection settings)
      {
      }

      /// <summary>
      ///   Gets or sets the name of the provider.
      /// </summary>
      public string Name
      {
         get { return name; }
         set { name = value; }
      }

      /// <summary>
      ///   Gets or sets a short description of the provider.
      /// </summary>
      public string Description
      {
         get { return description; }
         set { description = value; }
      }


      #endregion

      internal class QueryContext : XsltContext
      {
         public XPathNodeIterator current;
         public VariableList variables = new VariableList();

         public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
         {
            log.Error(String.Format("'{0}:{1}' is not defined as a function.", prefix, name));
            return null;
         }

         public override IXsltContextVariable ResolveVariable(string prefix, string name)
         {
            if (log.IsDebugEnabled)
               log.Debug(String.Format("Resolving variable {0}:{1}", prefix, name));

            string namespaceUri = this.LookupNamespace(prefix);
            IXsltContextVariable variable;
            if (string.IsNullOrEmpty(namespaceUri))
               variable = new VariableReference(name);
            else
               variable = null;
            if (variable == null)
               log.Error(String.Format("'{0}:{1}' is not defined as a variable.", prefix, name));

            return variable;
         }

         public override int CompareDocument(string baseUri, string nextbaseUri)
         {
            return 0;
         }
         public override bool PreserveWhitespace(XPathNavigator node)
         {
            return false;
         }
         public override bool Whitespace
         {
            get { return false; }
         }
      }

      internal class Variable : IXsltContextVariable
      {
         public string name;
         public string value;
         XPathExpression expression;
         public int scopeLevel;

         public Variable(string name, string value, int scopeLevel)
         {
            this.name = name;
            this.value = value;
            this.scopeLevel = scopeLevel;
         }
         #region IXsltContextVariable Members

         public object Evaluate(XsltContext xsltContext)
         {
            QueryContext qcontext = (QueryContext) xsltContext;

            if (expression == null)
            {
               expression = XPathExpression.Compile(value);
               expression.SetContext(xsltContext);
            }
            return qcontext.current.Current.Evaluate(expression);
         }

         public bool IsLocal
         {
            get { return false; }
         }

         public bool IsParam
         {
            get { return false; }
         }

         public XPathResultType VariableType
         {
            get { return XPathResultType.Any; }
         }

         #endregion
      }

      class VariableReference : IXsltContextVariable
      {
         string name;

         public VariableReference(string name)
         {
            this.name = name;
         }

         #region IXsltContextVariable Members

         public object Evaluate(XsltContext xsltContext)
         {
            QueryContext context = (QueryContext)xsltContext;
            Variable v = context.variables.Find(this.name);
            if (v == null)
            {
                  log.Error(String.Format("'{0}' is not defined as a variable.", name));
                  return null;
            }

            return v.Evaluate(xsltContext);
         }

         public bool IsLocal
         {
            get { return false; }
         }

         public bool IsParam
         {
            get { return false; }
         }

         public XPathResultType VariableType
         {
            get { return XPathResultType.Any; }
         }

         #endregion
      }

      internal class VariableList : List<Variable>
      {
         int scopeLevel;

         public Variable Add(string name, string value)
         {
            for (int i = Count - 1; 0 <= i && this[i].scopeLevel == scopeLevel; --i)
            {
               if (this[i].name == name)
                  throw new ArgumentException(String.Format("'{0}' is already defined.", name));
            }
            
            Variable variable = new Variable(name, value, scopeLevel);
            this.Add(variable);
            return variable;
         }

         public Variable Find(string name)
         {
            for (int i = Count - 1; 0 <= i; --i)
            {
               Variable variable = this[i];
               if (variable.name == name)
                  return variable;
            }

            return null;
         }

         public void PushScope()
         {
            ++scopeLevel;
         }

         public void PopScope()
         {
            if (scopeLevel == 0)
               throw new InvalidOperationException("The variable scope has not been pushed.");

            for (int i = Count - 1; 0 <= i && this[i].scopeLevel == scopeLevel; --i)
            {
               this.RemoveAt(i);
            }

            --scopeLevel;
         }

      }

   }
}
