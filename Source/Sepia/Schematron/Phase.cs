using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Serialization;

// TODO: foreign
// TODO: inclusion
namespace Sepia.Schematron
{
   /// <summary>
   ///   A named collection of <see cref="Pattern"/> objects.
   /// </summary>
   /// <remarks>
   ///   A <see cref="Pattern"/> may belong to more than one <see cref="Phase"/>. The phases names "#ALL" and "#DEFAULT" are
   ///   reserved.
   /// </remarks>
   [Serializable]
   public class Phase
   {
      /// <summary>
      ///   The reserved name, '#ALL', for all phases.
      /// </summary>
      public const string All = "#ALL";

      /// <summary>
      ///   The reserved name, '#DEFAULT', for the default phase(s).
      /// </summary>
      public const string Default = "#DEFAULT";

      private Annotation annotation;
      private ActivePatternCollection activePatterns;
      private string id;
      private string fpi;
      private string see;
      private string icon;
      private NameValueCollection parameters;

      /// <summary>
      ///   The collection of <see cref="Pattern"/> objects that are run by the <see cref="Phase"/>.
      /// </summary>
      /// <value>
      ///   An <see cref="ActivePatternCollection"/>.  The default value is an empty collection.
      /// </value>
      public ActivePatternCollection ActivePatterns
      {
         get
         {
            if (activePatterns == null)
               activePatterns = new ActivePatternCollection();

            return activePatterns;
         }
         set
         {
            activePatterns = value;
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
      ///   Gets or sets the unique identifier.
      /// </summary>
      public string ID
      {
         get
         {
            return id;
         }
         set
         {
            id = value;
         }
      }

      /// <summary>
      ///   A summary of the purpose or role of the <see cref="SchematronDocument"/>, for the purpose of documentation or a rich user interface.
      /// </summary>
      /// <value>
      ///   The default value is an empty <see cref="Annotation"/>.
      /// </value>
      public Annotation Annotation
      {
         get
         {
            if (annotation == null)
               annotation = new Annotation();

            return annotation;
         }
         set
         {
            annotation = value;
         }
      }
      /// <summary>
      ///   A formal public identifier for the object.
      /// </summary>
      public string Fpi
      {
         get
         {
            return fpi;
         }
         set
         {
            fpi = value;
         }
      }

      /// <summary>
      ///   The URI of external information of interest to maintainers and users of the schema.
      /// </summary>
      public string See
      {
         get
         {
            return see;
         }
         set
         {
            see = value;
         }
      }

      /// <summary>
      ///  The URI of an image containing some visible representation of the severity, significance or other grouping
      ///  of the associated object.
      /// </summary>
      public string Icon
      {
         get
         {
            return icon;
         }
         set
         {
            icon = value;
         }
      }

   }

   /// <summary>
   ///   A list of <see cref="Phase"/> objects.
   /// </summary>
   public class PhaseCollection : List<Phase>
   {
      /// <summary>
      ///   Get the <see cref="Phase"/> with the specified <see cref="Phase.ID"/>.
      /// </summary>
      /// <param name="id">
      ///   The unique identifier of the <see cref="Phase"/> to get.
      /// </param>
      /// <returns>
      ///   The <see cref="Phase"/> with the specified <paramref name="id"/>.
      /// </returns>
      public Phase this[string id]
      {
         get
         {
            foreach (Phase phase in this)
            {
               if (phase.ID == id)
                  return phase;
            }

            throw new ArgumentException(String.Format("'{0}' is not a defined phase.", id));
         }
      }
   }
}
