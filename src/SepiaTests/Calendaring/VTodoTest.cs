using Sepia.Calendaring.Serialization;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Unit tests for <see cref="VTodo"/>.
    /// </summary>
    [TestClass]
    public class VTodoTest
    {
        const string Crlf = "\r\n";

        /// <summary>
        ///   The default values for properties are defined when an instance is created.
        /// </summary>
        [TestMethod]
        public void Defaults()
        {
            var t0 = new VTodo();
            var t1 = new VTodo();
            Assert.AreNotEqual(t0.Id, t1.Id, "id is not unique");
            Assert.IsFalse(t0.StartsOn.HasValue, "starts on");
            Assert.AreEqual(AccessClassification.Public, t0.Classification);
            Assert.AreEqual(0, t0.Revision);
        }

        [TestMethod]
        public void Reading()
        {
            const string ics =
                "BEGIN:VCALENDAR" + Crlf +
                    "VERSION:2.0" + Crlf +
                "BEGIN:VTODO" + Crlf +
                   "UID:20070514T103211Z-123404@example.com" + Crlf +
                   "DTSTAMP:20070514T103211Z" + Crlf +
                   "DTSTART:20070514T110000Z" + Crlf +
                   "DUE:20070709T130000Z" + Crlf +
                   "COMPLETED:20070707T100000Z" + Crlf +
                   "SUMMARY:Submit Revised Internet-Draft" + Crlf +
                   "PRIORITY:1" + Crlf +
                   "CATEGORIES:WORK,IETF" + Crlf +
                   "STATUS:NEEDS-ACTION" + Crlf +
                "END:VTODO" + Crlf +
                "END:VCALENDAR" + Crlf;

            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(ics)));
            var todo = calendar.Components.OfType<VTodo>().First();

            Assert.AreEqual("20070514T103211Z-123404@example.com", todo.Id);
            Assert.AreEqual(new DateTime(2007, 05, 14, 10, 32, 11, DateTimeKind.Utc), todo.CreatedOnByAgent.Value);
            Assert.AreEqual(new DateTime(2007, 05, 14, 11, 00, 00, DateTimeKind.Utc), todo.StartsOn.Value.Value);
            Assert.AreEqual(new DateTime(2007, 07, 09, 13, 00, 00, DateTimeKind.Utc), todo.DueOn.Value.Value);
            //TODO Assert.AreEqual("", todo.CompletedOn.Value);
            // TODO: percent
            Assert.AreEqual("Submit Revised Internet-Draft", todo.Summary);
            Assert.AreEqual(1, todo.Priority);
            Assert.AreEqual(2, todo.Categories.Count);
            Assert.AreEqual(TodoStatus.NeedsAction, todo.Status);
        }

        [TestMethod]
        public void Writing()
        {
            const string ics0 =
                "BEGIN:VCALENDAR" + Crlf +
                    "VERSION:2.0" + Crlf +
                "BEGIN:VTODO" + Crlf +
                   "UID:20070514T103211Z-123404@example.com" + Crlf +
                   "DTSTAMP:20070514T103211Z" + Crlf +
                   "DTSTART:20070514T110000Z" + Crlf +
                   "DUE:20070709T130000Z" + Crlf +
                   "COMPLETED:20070707T100000Z" + Crlf +
                   "SUMMARY:Submit Revised Internet-Draft" + Crlf +
                   "PRIORITY:1" + Crlf +
                   "CATEGORIES:WORK,IETF" + Crlf +
                   "STATUS:NEEDS-ACTION" + Crlf +
                   "BEGIN:VALARM" + Crlf +
                        "TRIGGER;RELATED=END:-P2D" + Crlf +
                        "ACTION:EMAIL" + Crlf +
                        "ATTENDEE:mailto:john_doe@example.com" + Crlf +
                        "SUMMARY:*** REMINDER: SEND AGENDA FOR WEEKLY STAFF MEETING ***" + Crlf +
                        "DESCRIPTION:A draft agenda needs to be sent out to the attendees " + Crlf +
                        " to the weekly managers meeting (MGR-LIST). Attached is a " + Crlf +
                        " pointer the document template for the agenda file." + Crlf +
                        "ATTACH;FMTTYPE=application/msword:http://example.com/" + Crlf +
                        " templates/agenda.doc" + Crlf +
                   "END:VALARM" + Crlf +
                "END:VTODO" + Crlf +
                "END:VCALENDAR" + Crlf;

            var calendar0 = new VCalendar();
            calendar0.ReadIcs(IcsReader.Create(new StringReader(ics0)));
            var ics = new StringWriter();
            calendar0.WriteIcs(IcsWriter.Create(ics));
            Console.WriteLine(ics.ToString());
            var calendar1 = new VCalendar();
            calendar1.ReadIcs(IcsReader.Create(new StringReader(ics.ToString())));

            var todo = calendar1.Components.OfType<VTodo>().First();
            Assert.AreEqual("20070514T103211Z-123404@example.com", todo.Id);
            Assert.AreEqual(new DateTime(2007, 05, 14, 10, 32, 11, DateTimeKind.Utc), todo.CreatedOnByAgent.Value);
            Assert.AreEqual(new DateTime(2007, 05, 14, 11, 00, 00, DateTimeKind.Utc), todo.StartsOn.Value.Value);
            Assert.AreEqual(new DateTime(2007, 07, 09, 13, 00, 00, DateTimeKind.Utc), todo.DueOn.Value.Value);
            //TODO Assert.AreEqual("", todo.CompletedOn.Value);
            // TODO: percent
            Assert.AreEqual("Submit Revised Internet-Draft", todo.Summary);
            Assert.AreEqual(1, todo.Priority);
            Assert.AreEqual(2, todo.Categories.Count);
            Assert.AreEqual(TodoStatus.NeedsAction, todo.Status);
            Assert.IsNotNull(todo.Alarm, "missing alarm");
        }

    }
}
