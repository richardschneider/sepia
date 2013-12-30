using System;
using System.Collections.Generic;
using System.Xml.Serialization;

// TODO: foreign attributes

namespace Sepia.Schematron
{

   /// <summary>
   ///  Defines a namespace <see cref="Prefix"/> and <see cref="Uri"/>.
   /// </summary>
   /// <remarks>
   ///   A <see cref="Prefix"/> can be used in a <see cref="Rule.Context"/> or 
   ///   <see cref="Assertion.Test"/>.
   /// </remarks>
   [Serializable]
   public sealed class NamespaceDefinition
   {
      private string uri;
      private string prefix;

      /// <summary>
      ///   The URI of the namespace.
      /// </summary>
      public string Uri
      {
         get
         {
            return uri;
         }
         set
         {
            uri = value;
         }
      }

      /// <summary>
      ///   The prefix of the namespace.
      /// </summary>
      public string Prefix
      {
         get
         {
            return prefix;
         }
         set
         {
            prefix = value;
         }
      }
   }

   /// <summary>
   ///   A collection of <see cref="NamespaceDefinition"/> objects;
   /// </summary>
   public class NamespaceDefinitionCollection : List<NamespaceDefinition>
   {
   }
}