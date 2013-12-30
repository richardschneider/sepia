using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sepia.Calendaring.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring
{
    [TestClass]
    public class CalendarAttachmentTest
    {
        [TestMethod]
        public void Reading()
        {
            var attachment = new CalendarAttachment(new ContentLine("ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud"));
            Assert.AreEqual(null, attachment.Content);
            Assert.AreEqual("audio/basic", attachment.ContentType);
            Assert.AreEqual("ftp://example.com/pub/sounds/bell-01.aud", attachment.Uri);

            attachment = new CalendarAttachment(new ContentLine("ATTACH:CID:jsmith.part3.960817T083000.xyzMail@example.com"));
            Assert.AreEqual(null, attachment.Content);
            Assert.AreEqual(null, attachment.ContentType);
            Assert.AreEqual("CID:jsmith.part3.960817T083000.xyzMail@example.com", attachment.Uri);

            attachment = new CalendarAttachment(new ContentLine("ATTACH;FMTTYPE=text/plain;ENCODING=BASE64;VALUE=BINARY:VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZy4="));
            Assert.AreEqual("The quick brown fox jumps over the lazy dog.", Encoding.ASCII.GetString(attachment.Content));
            Assert.AreEqual("text/plain", attachment.ContentType);
            Assert.AreEqual(null, attachment.Uri);
        }

        [TestMethod]
        public void Writing() 
        {
            var attachment = new CalendarAttachment()
            {
                ContentType = "audio/basic",
                Uri = "ftp://example.com/pub/sounds/bell-01.aud"
            };
            Assert.AreEqual("ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud", attachment.ToString());

            attachment = new CalendarAttachment()
            {
                Uri = "CID:jsmith.part3.960817T083000.xyzMail@example.com"
            };
            Assert.AreEqual("ATTACH:CID:jsmith.part3.960817T083000.xyzMail@example.com", attachment.ToString());

            attachment = new CalendarAttachment()
            {
                ContentType = "text/plain",
                Content = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog.")
            };
            Assert.AreEqual("ATTACH;FMTTYPE=text/plain;ENCODING=BASE64;VALUE=BINARY:VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZy4=", attachment.ToString());
        }

    }
}
