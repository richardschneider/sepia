using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{
    using System.IO;

    [TestClass]
    public class RelationshipReferenceTest
    {
        [TestMethod]
        public void Defaults()
        {
            Assert.AreEqual(Relationship.Parent, new RelationshipReference().Relationship);
        }

        /// <summary>
        ///   A <see cref="RelationshipReference"/> can be deserialised from a <see cref="ContentLine"/>.
        /// </summary>
        [TestMethod]
        public void FromContentLine()
        {
            var rref = new RelationshipReference(new ContentLine("RELATED-TO:jsmith.part7.19960817T083000.xyzMail@example.com"));
            Assert.AreEqual("jsmith.part7.19960817T083000.xyzMail@example.com", rref.OtherUri);
            Assert.AreEqual(Relationship.Parent, rref.Relationship);

            rref = new RelationshipReference(new ContentLine("RELATED-TO;RELTYPE=SIBLING:19960401-080045-4000F192713@example.com"));
            Assert.AreEqual("19960401-080045-4000F192713@example.com", rref.OtherUri);
            Assert.AreEqual(Relationship.Sibling, rref.Relationship);
        }

        /// <summary>
        ///   A <see cref="RelationshipReference"/> can be serialised.
        /// </summary>
        [TestMethod]
        public void Serialisation()
        {
            var rref = WriteRead("RELATED-TO:jsmith.part7.19960817T083000.xyzMail@example.com");
            Assert.AreEqual("jsmith.part7.19960817T083000.xyzMail@example.com", rref.OtherUri);
            Assert.AreEqual(Relationship.Parent, rref.Relationship);

            rref = WriteRead("RELATED-TO;RELTYPE=SIBLING:19960401-080045-4000F192713@example.com");
            Assert.AreEqual("19960401-080045-4000F192713@example.com", rref.OtherUri);
            Assert.AreEqual(Relationship.Sibling, rref.Relationship);
        }

        RelationshipReference WriteRead(string ics)
        {
            var rref = new RelationshipReference(new ContentLine(ics));
            var s = new StringWriter();
            rref.WriteIcs(IcsWriter.Create(s));

            return new RelationshipReference(new ContentLine(s.ToString()));
        }

    }
}
