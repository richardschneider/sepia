using Sepia.Calendaring.Serialization;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring
{
    [TestClass]
    public class RequestStatusTest
    {
        /// <summary>
        ///   A <see cref="RequestStatus"/> can be deserialised from a <see cref="ContentLine"/>.
        /// </summary>
        [TestMethod]
        public void FromContentLine()
        {
            var status = new RequestStatus(new ContentLine("REQUEST-STATUS:2.0;Success"));
            Assert.AreEqual("2.0", status.Code);
            Assert.AreEqual("Success", status.Description);
            Assert.AreEqual(LanguageTag.Unspecified, status.Description.Language);
            Assert.AreEqual(null, status.RelatedData);
            Assert.IsTrue(status.IsSuccess);

            status = new RequestStatus(new ContentLine("REQUEST-STATUS:3.1;Invalid property value;DTSTART:96-Apr-01"));
            Assert.AreEqual("3.1", status.Code);
            Assert.AreEqual("Invalid property value", status.Description);
            Assert.AreEqual(LanguageTag.Unspecified, status.Description.Language);
            Assert.AreEqual("DTSTART:96-Apr-01", status.RelatedData);
            Assert.IsTrue(status.IsClientError);

            status = new RequestStatus(new ContentLine("REQUEST-STATUS;LANGUAGE=en:3.1;Invalid property value;DTSTART:96-Apr-01"));
            Assert.AreEqual("3.1", status.Code);
            Assert.AreEqual("Invalid property value", status.Description);
            Assert.AreEqual("en", status.Description.Language);
            Assert.AreEqual("DTSTART:96-Apr-01", status.RelatedData);
            Assert.IsTrue(status.IsClientError);

            status = new RequestStatus(new ContentLine(@"REQUEST-STATUS:2.8; Success\, repeating event ignored. Scheduled as a single event.;RRULE:FREQ=WEEKLY;INTERVAL=2"));
            Assert.AreEqual("2.8", status.Code);
            Assert.AreEqual("Success, repeating event ignored. Scheduled as a single event.", status.Description);
            Assert.AreEqual(LanguageTag.Unspecified, status.Description.Language);
            Assert.AreEqual("RRULE:FREQ=WEEKLY;INTERVAL=2", status.RelatedData);
            Assert.IsTrue(status.IsSuccess);
        }

        [TestMethod]
        public void Writing()
        {
            var status = new RequestStatus()
            {
                Code = "3.1",
                Description = new Text("en", "Invalid property value"),
                RelatedData = "DTSTART:96-Apr-01"
            };
            Assert.AreEqual("REQUEST-STATUS;LANGUAGE=en:3.1;Invalid property value;DTSTART:96-Apr-01", status.ToString());

            status = new RequestStatus()
            {
                Code = "3.1",
                Description = new Text(LanguageTag.Unspecified, "Invalid property value"),
                RelatedData = "DTSTART:96-Apr-01"
            };
            Assert.AreEqual("REQUEST-STATUS:3.1;Invalid property value;DTSTART:96-Apr-01", status.ToString());

            status = new RequestStatus()
            {
                Code = "3.1",
                Description = new Text(LanguageTag.Unspecified, "Invalid property value")
            };
            Assert.AreEqual("REQUEST-STATUS:3.1;Invalid property value", status.ToString());

            status = new RequestStatus()
            {
                Code = "3.1"
            };
            Assert.AreEqual("REQUEST-STATUS:3.1", status.ToString());

            status = new RequestStatus()
            {
                Code = "2.8",
                Description = new Text(LanguageTag.Unspecified, "Success, repeating event ignored. Scheduled as a single event."),
                RelatedData = "RRULE:FREQ=WEEKLY;INTERVAL=2"
            };
            Assert.AreEqual(@"REQUEST-STATUS:2.8;Success\, repeating event ignored. Scheduled as a single event.;RRULE:FREQ=WEEKLY;INTERVAL=2", status.ToString());
        }
    }
}
