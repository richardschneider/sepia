using Sepia.Schematron.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepia.Schematron
{
   /// <summary>
   ///   Compiles a <see cref="SchematronDocument"/>.
   /// </summary>
   /// <remarks>
   ///   <b>Compiler</b> produces a "minimal syntax" schematron document, as specifed in Section 6.2 of ISO/IEC 19757-3.  It performs
   ///   the following transformations:
   ///   <list type="bullet">
   ///   <item>Resolve all inclusions by replacing the include element by the resource linked to.</item>
   ///   <item>Resolve all abstract <see cref="Pattern">patterns</see> by replacing parameter references with actual parameter values in 
   ///   all enclosed attributes that contain queries.</item>
   ///   <item>Resolve all abstract <see cref="Rule">rules</see> by replacing the <see cref="Extends"/> with 
   ///         the contents of the abstract rule identified.</item>
   ///   <item>Negate all <see cref="Report"/> objects into <see cref="Assertion"/> objects.</item>
   ///   </list>
   /// </remarks>
   public class Compiler
   {
      SchematronDocument schematron;
      SchematronDocument minimal;

      /// <summary>
      ///   Produces a "minimal syntax" schematron from the specified <see cref="SchematronDocument"/>.
      /// </summary>
      /// <param name="schematron">A <see cref="SchematronDocument"/>.</param>
      /// <returns>
      ///   A minmal syntax <see cref="SchematronDocument"/> representation of the <paramref name="schematron"/>.
      /// </returns>
      public SchematronDocument Compile(SchematronDocument schematron)
      {
         if (schematron == null)
            throw new ArgumentNullException("schematron");

         this.schematron = schematron;
         minimal = new SchematronDocument();

         CopyDocument();
         ResolveInclusions();
         ResolvePhases();
         ResolvePatterns();

         return minimal;
      }

      void CopyDocument()
      {
         minimal.Title = schematron.Title;
         minimal.DefaultPhase = schematron.DefaultPhase;
         minimal.Diagnostics = schematron.Diagnostics;
         minimal.ID = schematron.ID;
         minimal.Namespaces = schematron.Namespaces;
         minimal.QueryLanguage = schematron.QueryLanguage;
         minimal.SchemaVersion = schematron.SchemaVersion;
         if (schematron.HasParameters)
            minimal.Parameters = schematron.Parameters;
      }

      void ResolveInclusions() // TODO
      {
      }

      void ResolvePhases()
      {
         if (schematron.HasPhases)
         {
            foreach (Phase phase in schematron.Phases)
            {
               Phase minimalPhase = new Phase();
               minimalPhase.ActivePatterns = phase.ActivePatterns;
               minimalPhase.ID = phase.ID;
               minimal.Phases.Add(minimalPhase);
               if (phase.HasParameters)
                  minimalPhase.Parameters = phase.Parameters;
            }
         }
      }

      void ResolvePatterns()
      {
         // Resolve all the rules.
         foreach (Pattern pattern in schematron.Patterns)
         {
            Pattern minimalPattern = new Pattern();
            minimalPattern.IsAbstract = pattern.IsAbstract;
            minimalPattern.BasePatternID = pattern.BasePatternID;
            minimalPattern.ID = pattern.ID;
            minimalPattern.Title = pattern.Title;
            if (pattern.HasParameters)
               minimalPattern.Parameters = pattern.Parameters;
            minimal.Patterns.Add(minimalPattern);
            ResolveRules(pattern, minimalPattern);
         }

         // Add the rules for is-a patterns and use the parameters for each rule.
         foreach (Pattern pattern in minimal.Patterns)
         {
            if (!string.IsNullOrEmpty(pattern.BasePatternID))
            {
               foreach (Rule rule in minimal.Patterns[pattern.BasePatternID].Rules)
               {
                  Rule cloned = rule.Clone();
                  pattern.Rules.Add(cloned);
                  cloned.Parameters.Add(pattern.Parameters);
               }
               pattern.BasePatternID = "";
               pattern.Parameters.Clear();
            }
         }

         // Remove the abstract patterns.
         for (int i = minimal.Patterns.Count - 1; 0 <= i; --i)
         {
            if (minimal.Patterns[i].IsAbstract)
               minimal.Patterns.RemoveAt(i);
         }
      }

      void ResolveRules(Pattern fullPattern, Pattern minimalPattern)
      {
         foreach (Rule rule in fullPattern.Rules)
         {
            if (!rule.IsAbstract)
            {
               Rule extendedRule;
               if (rule.HasExtensions)
               {
                  extendedRule = new Rule();
                  extendedRule.Context = rule.Context;
                  extendedRule.ID = rule.ID;
                  extendedRule.IsAbstract = rule.IsAbstract;

                  AddAssertions(fullPattern, extendedRule, rule);
                  extendedRule.Assertions.AddRange(rule.Assertions);
               }
               else
               {
                  extendedRule = rule;
               }

               minimalPattern.Rules.Add(extendedRule);
            }
         }
      }

      void AddAssertions(Pattern pattern, Rule extendedRule, Rule rule)
      {
         foreach (Extends extends in rule.Extends)
         {
            string id = extends.RuleID;
            Rule r = pattern.Rules[id];
            if (r.HasExtensions)
               AddAssertions(pattern, extendedRule, r);
            extendedRule.Assertions.AddRange(r.Assertions);
         }

      }


   }
}

