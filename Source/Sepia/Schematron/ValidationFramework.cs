using Sepia.Schematron.Queries;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Sepia.Schematron
{

   /// <summary>
   ///   Represents the method that will handle any schematron validation events.
   /// </summary>
   /// <param name="sender">
   ///   The source of the event.
   /// </param>
   /// <param name="e">
   ///   A <see cref="SchematronValidationEventArgs"/> than contains the event data.
   /// </param>
   public delegate void SchematronValidationEventHandler(object sender, SchematronValidationEventArgs e);

   /// <summary>
   ///   Provides data for any validation events.
   /// </summary>
   /// <remarks>
   ///   A <b>SchematronValidationEventArgs</b> is created when a validator
   ///   detects an error or warning condition.
   /// </remarks>
   [Serializable]
   public sealed class SchematronValidationEventArgs : EventArgs
   {
      SchematronDocument schematron;
      IQueryLanguage queryEngine;
      Pattern pattern;
      Rule rule;
      Assertion assertion;
      XPathNavigator instance;
      string message;
      string[] diagnostics;

      /// <summary>
      ///   Creates a new instance of the <see cref="SchematronValidationEventArgs"/>.
      /// </summary>
      /// <param name="schematron">The <see cref="SchematronDocument"/> that detected the event.</param>
      /// <param name="queryEngine">The <see cref="IQueryLanguage"/> that detected the event.</param>
      /// <param name="pattern">The active <see cref="Pattern"/>.</param>
      /// <param name="rule">The <see cref="Sepia.Schematron.Rule"/> that caused the event to be raised.</param>
      /// <param name="assertion">The <see cref="Sepia.Schematron.Assertion"/> that caused the event to be raised.</param>
      /// <param name="context">An <see cref="object"/> that provides the context for the <paramref name="rule"/> and <paramref name="assertion"/>.</param>
      /// <param name="instance">An <see cref="XPathNavigator"/> to the document node that cause the event to be raised.</param>
      public SchematronValidationEventArgs(SchematronDocument schematron, IQueryLanguage queryEngine, Pattern pattern, Rule rule, Assertion assertion, object context, XPathNavigator instance)
      {
         this.schematron = schematron;
         this.queryEngine = queryEngine;
         this.pattern = pattern;
         this.rule = rule;
         this.assertion = assertion;
         this.instance = instance.Clone();

         if (assertion == null)
         {
            message = "A schematron validation event occured.";
         }
         else
         {
            message = assertion.Message.ToString(instance, context);
         }

         List<string> diagnostics = new List<string>();
         if (assertion != null && !string.IsNullOrEmpty(assertion.Diagnostics))
         {
            foreach (string id in assertion.Diagnostics.Split(' '))
            {
               Diagnostic diagnostic = schematron.Diagnostics[id];
               diagnostics.Add(diagnostic.Message.ToString(instance, context));
            }
         }
         this.diagnostics = diagnostics.ToArray();
      }


      /// <summary>
      ///   Gets the message associated with the event.
      /// </summary>
      public string Message
      {
         get { return message; }
      }

      /// <summary>
      ///   Gets the diagnostic messages associated with the event.
      /// </summary>
      /// <value>
      ///   A list of diagnostic messages.  If no <see cref="Sepia.Schematron.Assertion.Diagnostics"/> exists, then an empty
      ///   list is returned.
      /// </value>
      public string[] Diagnostics
      {
         get
         {
            return diagnostics;
         }
      }

      /// <summary>
      ///   Gets the <see cref="SchematronDocument"/> that detected the event.
      /// </summary>
      public SchematronDocument Schematron
      {
         get { return schematron; }
      }

      /// <summary>
      ///   Gets the <see cref="IQueryLanguage"/> that detected the event.
      /// </summary>
      public IQueryLanguage QueryEngine
      {
         get { return queryEngine; }
      }

      /// <summary>
      ///   Gets the active <see cref="Pattern"/>.
      /// </summary>
      public Pattern Pattern
      {
         get { return pattern; }
      }

      /// <summary>
      ///   Get the <see cref="Sepia.Schematron.Rule"/> that caused the event to be raised.
      /// </summary>
      public Rule Rule
      {
         get { return rule; }
      }

      /// <summary>
      ///   Gets The <see cref="Sepia.Schematron.Assertion"/> that caused the event to be raised.
      /// </summary>
      public Assertion Assertion
      {
         get { return assertion; }
      }

      /// <summary>
      ///   An <see cref="XPathNavigator"/> to the document node that cause the event to be raised.
      /// </summary>
      public XPathNavigator Instance
      {
         get { return instance; }
      }
   }



   ///<summary>
   ///  The exception that is thrown when there is no <see cref="SchematronValidationEventHandler"/>.
   ///</summary>
   [Serializable]
   public class SchematronValidationException : Exception
   {

      ///<summary>
      ///  Initializes a new instance of the <see cref="SchematronValidationException"/> class.
      ///</summary>
      ///<remarks>
      ///  This constructor initializes the <see cref="Exception.Message"/> property of the new 
      ///  instance to a system-supplied message that describes the error and takes into 
      ///  account the current system culture.
      ///</remarks>
      public SchematronValidationException()
         : base()
      { }

      /// <summary>
      ///   Initializes a new instance of the <see cref="SchematronValidationException"/> class with a specified error message.
      /// </summary>
      /// <param name="message">
      ///   The error message that explains the reason for the exception.
      /// </param>
      /// <remarks>
      ///   This constructor initializes the <see cref="Exception.Message"/> property of the new 
      ///   instance to the <paramref name="message"/> parameter.
      /// </remarks>
      public SchematronValidationException(string message)
         : base(message)
      {
      }

      /// <summary>
      ///   Initializes a new instance of the <see cref="SchematronValidationException"/> class with a specified error message and
      ///   inner <see cref="Exception"/>.
      /// </summary>
      /// <param name="message">
      ///   The error message that explains the reason for the exception.
      /// </param>
      /// <param name="innerException">
      ///   The <see cref="Exception"/> that is the cause of the current exception.
      /// </param>
      public SchematronValidationException(string message, Exception innerException)
         : base(message, innerException)
      {
      }

      /// <summary>
      ///   Initializes a new instance of te <see cref="SchematronValidationException"/> class with the specified
      ///   <see cref="SchematronValidationEventArgs"/>.
      /// </summary>
      /// <param name="e">
      ///   A <see cref="SchematronValidationEventArgs"/> that contains the details of the schematron validation event.
      /// </param>
      public SchematronValidationException(SchematronValidationEventArgs e)
         : base(e.Message)
      {
      }

      ///<summary>
      ///  Initializes a new instance of the <see cref="SchematronValidationException"/> class with serialized data.
      ///</summary>
      ///<param name="info">
      ///  The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
      ///</param>
      ///<param name="context">
      ///  The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
      ///</param>
      ///<remarks>
      ///  This constructor is called during deserialization to reconstitute the 
      ///  exception object transmitted over a stream. 
      ///</remarks>
      protected SchematronValidationException(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
      }

   }


}
