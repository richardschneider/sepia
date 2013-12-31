using Sepia.Schematron.Queries;
using Common.Logging;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Sepia.Schematron
{
   /// <summary>
   ///   Validates an <see cref="XmlDocument"/> against a <see cref="SchematronDocument"/>.
   /// </summary>
   /// <remarks>
   ///   The Validate method is used validate an <see cref="XmlDocument"/> against a <see cref="SchematronDocument"/>.  This
   ///   method, which is overloaded, allows either a <see cref="SchematronValidationException"/> to be <c>thrown</c> on the first error
   ///   or <see cref="SchematronValidationEventHandler"/> to be invoked on each error.
   /// <para>
   ///   The <see cref="ValidationPhase"/> property can be used to specify which <see cref="SchematronDocument.Phases">phase</see> is run.  The
   ///   default is to use the <see cref="SchematronDocument.DefaultPhase"/> property, which defaults to "#ALL". When it is equal to "#ALL"
   ///   all <see cref="SchematronDocument.Patterns"/> are run.
   /// </para>
   /// </remarks>
   /// <example>
   /// <code>
   ///   SchematronValidator validator = new SchematronValidator("my.sch");
   ///   XmlDocument instance = XmlReader.Create("myinstance.xml");
   ///   validator.Validate(instance);
   /// </code>
   /// </example>
   /// <seealso cref="SchematronValidationException"/>
   /// <seealso cref="SchematronValidationEventArgs"/>
   /// <seealso cref="ValidationReport">Schematron Validation Report Language (SVRL)</seealso>
   public class SchematronValidator
   {
      static ILog log = LogManager.GetLogger(typeof(SchematronValidator));

      /// <summary>
      ///   Occurs when an <see cref="Assertion"/> fails.
      /// </summary>
      public event SchematronValidationEventHandler AssertionFailed;

      /// <summary>
      ///   Occurs at the start of a document validation.
      /// </summary>
      public event SchematronValidationEventHandler Start;

      /// <summary>
      ///   Occurs at the end of a document validation.
      /// </summary>
      public event SchematronValidationEventHandler End;

      /// <summary>
      ///   Occurs when a <see cref="Rule"/> fires, the <see cref="Rule.Context"/> is matched.
      /// </summary>
      public event SchematronValidationEventHandler RuleFired;

      /// <summary>
      ///   Occurs when an <see cref="Pattern"/> becomes active.
      /// </summary>
      public event SchematronValidationEventHandler ActivePattern;

      string phase = Sepia.Schematron.Phase.Default;
      SchematronDocument schematron;
      NameValueCollection parameters;
      XPathNavigator instanceNavigator;
      IQueryLanguage queryEngine;
      object queryContext;

      /// <summary>
      ///   Creates a new instance of the <see cref="SchematronValidator"/> class.
      /// </summary>
      public SchematronValidator()
      {
      }

      /// <summary>
      ///   Creates a new instance of the <see cref="SchematronValidator"/> class with the specified <see cref="SchematronDocument"/>.
      /// </summary>
      /// <param name="schemaDocument">
      ///   The <see cref="SchematronDocument"/> that will be used to validate an <see cref="XmlDocument"/>.
      /// </param>
      public SchematronValidator(SchematronDocument schemaDocument)
      {
         this.SchemaDocument = schemaDocument;
      }

      /// <summary>
      ///   Creates a new instance of the <see cref="SchematronValidator"/> class with the specified <see cref="SchematronDocument"/> URI.
      /// </summary>
      /// <param name="uri">
      ///   The URI of a <see cref="SchematronDocument"/> that will be used to validate an <see cref="XmlDocument"/>.
      /// </param>
      public SchematronValidator(string uri)
      {
         this.SchemaDocument = SchematronReader.ReadSchematron(uri);
      }

      /// <summary>
      ///   Gets or sets the <see cref="Sepia.Schematron.SchematronDocument"/> used for validation.
      /// </summary>
      public SchematronDocument SchemaDocument
      {
         get { return schematron; }
         set 
         {
            if (value == null)
               throw new ArgumentNullException();

            schematron = value.CompiledDocument; 
         }
      }

      /// <summary>
      ///   The parameters for the <see cref="Pattern"/>.
      /// </summary>
      /// <value>
      ///   A <see cref="NameValueCollection"/>.  The default value is an empty collection.
      /// </value>
      /// <seealso cref="HasParameters"/>
      /// <remarks>
      ///   Each parameter value is considered an expression and is evaluated by query language.  Thus, to use
      ///   a string constant the value should be enclosed in quotation marks.
      /// <para>
      ///   These parameters are used to override any <see cref="SchematronDocument"/> or <see cref="Phase"/> parameters.
      /// </para>
      /// </remarks>
      public NameValueCollection Parameters
      {
         get
         {
            if (parameters == null)
               parameters = new NameValueCollection();

            return parameters;
         }
         set
         {
            parameters = value;
         }
      }

      /// <summary>
      ///   Determines if any <see cref="Parameters"/> have been specified.
      /// </summary>
      /// <seealso cref="Parameters"/>
      public bool HasParameters
      {
         get { return parameters != null && parameters.Count > 0; }
      }

      /// <summary>
      ///   Gets or sets the <see cref="Phase.ID"/> of the <see cref="Phase"/> used for validation.
      /// </summary>
      /// <value>
      ///   The id of the <see cref="Phase"/> used for validation.  The default value is <see cref="Phase.Default"/>.
      /// </value>
      /// <remarks>
      ///   If schema does not specified a <see cref="SchematronDocument.DefaultPhase"/> then all <see cref="SchematronDocument.Patterns"/>
      ///   are used for validation.
      /// <para>
      ///   The "#ALL" can be used to specified all patterns in the schematron document.
      /// </para>
      /// </remarks>
      /// <see cref="Phase.Default"/>
      /// <see cref="Phase.All"/>
      public string ValidationPhase
      {
         get { return phase; }
         set { phase = value; }
      }

      /// <summary>
      ///   Validates the <see cref="XmlDocument"/>
      /// </summary>
      /// <param name="instance">The <see cref="XmlDocument"/> to validate.</param>
      /// <remarks>
      ///   <b>Validate</b> validates the <paramref name="instance"/> against the <see cref="SchemaDocument"/>.  
      /// <para>
      ///   If an error is detected and
      ///   the <see cref="AssertionFailed"/> event is <b>null</b>, then a <see cref="SchematronValidationException"/>is <c>thrown</c>.
      ///   Otherwise, the <see cref="AssertionFailed"/> event is raised.
      /// </para>
      /// </remarks>
      public void Validate(XmlDocument instance)
      {
         Validate(instance, null);
      }

      /// <summary>
      ///   Validates the <see cref="XmlDocument"/>
      /// </summary>
      /// <param name="instance">The <see cref="XmlDocument"/> to validate.</param>
      /// <param name="handler">A <see cref="SchematronValidationEventHandler"/> to receive errors.</param>
      /// <remarks>
      ///   <b>Validate</b> validates the <paramref name="instance"/> against the <see cref="SchemaDocument"/>.  
      /// <para>
      ///   If an error is detected and
      ///   the <see cref="AssertionFailed"/> event is <b>null</b>, then a <see cref="SchematronValidationException"/>is <c>thrown</c>.
      ///   Otherwise, the <see cref="AssertionFailed"/> event is raised.
      /// </para>
      /// </remarks>
      public void Validate(XmlDocument instance, SchematronValidationEventHandler handler)
      {
         if (instance == null)
            throw new ArgumentNullException("instance");
         if (schematron == null)
            throw new InvalidOperationException("The schematron document is not specified.");
         if (SchemaDocument.Patterns == null || SchemaDocument.Patterns.Count == 0)
            throw new InvalidOperationException("The schematron document has no patterns.");
         
         if (handler != null)
            AssertionFailed += handler;

        if (log.IsInfoEnabled && !string.IsNullOrEmpty(instance.BaseURI))
            log.Info("Validating " + instance.BaseURI);

         // Bind to the query language.
         if (!Schematron.Default.QueryLanguages.Providers.ContainsKey(schematron.QueryLanguage))
            throw new InvalidOperationException(String.Format("'{0}' is an unknown query language.", schematron.QueryLanguage));
         queryEngine = Schematron.Default.QueryLanguages.Providers[schematron.QueryLanguage];
         queryContext = queryEngine.CreateMatchContext(schematron, instance);
         instanceNavigator = instance.CreateNavigator();

         // Apply the schema parameters.
         if (schematron.HasParameters)
         {
            foreach (string name in schematron.Parameters)
            {
               queryEngine.Let(queryContext, name, schematron.Parameters[name]);
            }
         }

         // Get the patterns to run.
         PatternCollection activePatterns;
         string phaseName = ValidationPhase;
         if (phaseName == Phase.Default)
            phaseName = schematron.DefaultPhase;
         if (phaseName == Phase.All)
            activePatterns = schematron.Patterns;
         else
         {
            Phase phase = schematron.Phases[phaseName];
            activePatterns = new PatternCollection();
            foreach (ActivePattern activePattern in phase.ActivePatterns)
            {
               activePatterns.Add(schematron.Patterns[activePattern.Pattern]);
            }
            // Apply the phase parameters.
            if (phase.HasParameters)
            {
               queryEngine.PushScope(queryContext);
               foreach (string name in phase.Parameters)
               {
                  queryEngine.Let(queryContext, name, phase.Parameters[name]);
               }
            }
         }

         // Apply the parameters specified for this validation.
         if (HasParameters)
         {
            queryEngine.PushScope(queryContext);
            foreach (string name in Parameters)
            {
               queryEngine.Let(queryContext, name, Parameters[name]);
            }
         }

         if (log.IsInfoEnabled)
            log.Info(String.Format("Schema = '{0}', phase = '{1}'", schematron.Title.ToString(), phaseName));
         if (Start != null)
            Start(this, new SchematronValidationEventArgs(schematron, queryEngine, null, null, null, queryContext, instanceNavigator));
         try
         {
            foreach (Pattern pattern in activePatterns)
            {
               if (log.IsInfoEnabled)
                  log.Info("Running pattern " + pattern.Title.ToString());
               if (ActivePattern != null)
                  ActivePattern(this, new SchematronValidationEventArgs(schematron, queryEngine, pattern, null, null, queryContext, instanceNavigator));

               // Apply the parameters
               queryEngine.PushScope(queryContext);
               if (pattern.HasParameters)
               {
                  foreach (string name in pattern.Parameters)
                  {
                     queryEngine.Let(queryContext, name, pattern.Parameters[name]);
                  }
               }

               // Run the pattern.
               bool q = RunPattern(pattern);
               queryEngine.PopScope(queryContext);

               if (log.IsInfoEnabled)
                  log.Info(String.Format("Pattern '{0}' {1}", pattern.Title.ToString(), q ? "succeeds" : "fails"));
            }
         }
         finally
         {
            if (End != null)
               End(this, new SchematronValidationEventArgs(schematron, queryEngine, null, null, null, queryContext, instanceNavigator));
         }
      }

      bool RunPattern(Pattern pattern)
      {
         bool ok = true;

         // Apply the pattern rules to the current node (context).  Only the first rule that matches is tested.
         foreach (Rule rule in pattern.Rules)
         {
            if (rule.IsAbstract)
               continue;

            // Apply the parameters
            queryEngine.PushScope(queryContext);
            if (rule.HasParameters)
            {
               foreach (string name in rule.Parameters)
               {
                  queryEngine.Let(queryContext, name, rule.Parameters[name]);
               }
            }

            // Perform the match
            bool matches = queryEngine.Match(rule, queryContext, instanceNavigator);
            if (!matches)
            {
               queryEngine.PopScope(queryContext);
               continue;
            }

            // Fire the rule and run the tests.
            if (log.IsDebugEnabled)
               log.Debug("Fired rule " + rule.Context);
            if (RuleFired != null)
               RuleFired(this, new SchematronValidationEventArgs(schematron, queryEngine, pattern, rule, null, queryContext, instanceNavigator));

            foreach (Assertion assertion in rule.Assertions)
            {
               bool q = queryEngine.Assert(assertion, queryContext, instanceNavigator);
               if (q && log.IsDebugEnabled)
                  log.Debug(String.Format("Test '{0}' succeeds", assertion.Test));
               else if (!q)
               {
                  OnValidationEvent(new SchematronValidationEventArgs(schematron, queryEngine, pattern, rule, assertion, queryContext, instanceNavigator));
                  ok = false;
                  break;
               }
            }

            queryEngine.PopScope(queryContext);
            break; // Only the first rule that matches the node is tested.
         }

         // Recurse on the children if no failure.
         if (ok && instanceNavigator.HasChildren)
         {
            instanceNavigator.MoveToFirstChild();
            do
            {
               if (!RunPattern(pattern))
                  ok = false;
            } while (instanceNavigator.MoveToNext());
            instanceNavigator.MoveToParent();
         }

         return ok;
      }

      void OnValidationEvent(SchematronValidationEventArgs e)
      {
         if (log.IsErrorEnabled)
            log.Error(String.Format("Test '{0}' fails at {1}", e.Assertion.Test, XPathHelper.FullName(e.Instance)));

         if (AssertionFailed != null)
            AssertionFailed(this, e);
         else
            throw new SchematronValidationException(e);

      }
   }
}
