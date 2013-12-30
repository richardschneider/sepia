using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring
{
    using System.IO;
    using Sepia.Calendaring.Serialization;

    [TestClass]
    public class ContactTest
    {
        [TestMethod]
        public void Reading()
        {
            var contact = new Contact(new ContentLine("CONTACT;ALTREP=\"http://example.com/pdi/jdoe.vcf\":Jim Dolittle\\, ABC Industries\\, +1-919-555-1234"));
            Assert.AreEqual("Jim Dolittle, ABC Industries, +1-919-555-1234", contact.Text);
            Assert.AreEqual("http://example.com/pdi/jdoe.vcf", contact.Uri);
        }

        [TestMethod]
        public void Writing()
        {
            var contact0 = new Contact(new ContentLine("CONTACT;ALTREP=\"http://example.com/pdi/jdoe.vcf\":Jim Dolittle\\, ABC Industries\\, +1-919-555-1234"));
            var ics = new StringWriter();
            contact0.WriteIcs(IcsWriter.Create(ics));
            var contact1 = new Contact(new ContentLine(ics.ToString()));
            Assert.AreEqual("Jim Dolittle, ABC Industries, +1-919-555-1234", contact1.Text);
            Assert.AreEqual("http://example.com/pdi/jdoe.vcf", contact1.Uri);
        }
    }
}
