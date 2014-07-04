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
    public class AttendeeTest
    {
        [TestMethod]
        public void Reading()
        {
            var attendee = new Attendee(new ContentLine("ATTENDEE;MEMBER=\"mailto:DEV-GROUP@example.com\",\"mailto:dev@org\":mailto:joecool@example.com"));
            Assert.AreEqual("joecool@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("DEV-GROUP@example.com", attendee.Membership.First().Address);
            Assert.AreEqual("dev@org", attendee.Membership.Last().Address);

            attendee = new Attendee(new ContentLine("ATTENDEE;DELEGATED-FROM=\"mailto:immud@example.com\":mailto:ildoit@example.com"));
            Assert.AreEqual("ildoit@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("immud@example.com", attendee.DelegatedFrom.First().Address);

            attendee = new Attendee(new ContentLine("ATTENDEE;DELEGATED-TO=\"mailto:jdoe@example.com\",\"mailto:jqpublic@example.com\":mailto:jsmith@example.com"));
            Assert.AreEqual("jsmith@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("jdoe@example.com", attendee.DelegatedTo.First().Address);
            Assert.AreEqual("jqpublic@example.com", attendee.DelegatedTo.Last().Address);

            attendee = new Attendee(new ContentLine("ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=TENTATIVE;CN=Henry Cabot:mailto:hcabot@example.com"));
            Assert.AreEqual("hcabot@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("Henry Cabot", attendee.MailAddress.DisplayName);
            Assert.AreEqual(UserRole.Required, attendee.UserRole);
            Assert.AreEqual(ParticipationStatus.Tentative, attendee.ParticipationStatus);

            attendee = new Attendee(new ContentLine("ATTENDEE;SENT-BY=\"mailto:jan_doe@example.com\";CN=John Smith:mailto:jsmith@example.com"));
            Assert.AreEqual("jsmith@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("John Smith", attendee.MailAddress.DisplayName);
            Assert.AreEqual("jan_doe@example.com", attendee.SentBy.Address);

            attendee = new Attendee(new ContentLine("ATTENDEE;CUTYPE=ROOM:mailto:board-room@example.com"));
            Assert.AreEqual("board-room@example.com", attendee.MailAddress.Address);
            Assert.AreEqual(UserType.Room, attendee.UserType);
        }

        [TestMethod]
        public void Writing()
        {
            var attendee = WriteRead("ATTENDEE;MEMBER=\"mailto:DEV-GROUP@example.com\",\"mailto:dev@org\":mailto:joecool@example.com");
            Assert.AreEqual("joecool@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("DEV-GROUP@example.com", attendee.Membership.First().Address);
            Assert.AreEqual("dev@org", attendee.Membership.Last().Address);

            attendee = WriteRead("ATTENDEE;DELEGATED-FROM=\"mailto:immud@example.com\":mailto:ildoit@example.com");
            Assert.AreEqual("ildoit@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("immud@example.com", attendee.DelegatedFrom.First().Address);

            attendee = WriteRead("ATTENDEE;DELEGATED-TO=\"mailto:jdoe@example.com\",\"mailto:jqpublic@example.com\":mailto:jsmith@example.com");
            Assert.AreEqual("jsmith@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("jdoe@example.com", attendee.DelegatedTo.First().Address);
            Assert.AreEqual("jqpublic@example.com", attendee.DelegatedTo.Last().Address);

            attendee = WriteRead("ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=TENTATIVE;CN=Henry Cabot:mailto:hcabot@example.com");
            Assert.AreEqual("hcabot@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("Henry Cabot", attendee.MailAddress.DisplayName);
            Assert.AreEqual(UserRole.Required, attendee.UserRole);
            Assert.AreEqual(ParticipationStatus.Tentative, attendee.ParticipationStatus);

            attendee = WriteRead("ATTENDEE;SENT-BY=\"mailto:jan_doe@example.com\";CN=John Smith:mailto:jsmith@example.com");
            Assert.AreEqual("jsmith@example.com", attendee.MailAddress.Address);
            Assert.AreEqual("John Smith", attendee.MailAddress.DisplayName);
            Assert.AreEqual("jan_doe@example.com", attendee.SentBy.Address);

            attendee = WriteRead("ATTENDEE;CUTYPE=ROOM:mailto:board-room@example.com");
            Assert.AreEqual("board-room@example.com", attendee.MailAddress.Address);
            Assert.AreEqual(UserType.Room, attendee.UserType);
        }

        Attendee WriteRead(string ics)
        {
            var attendee = new Attendee(new ContentLine(ics));
            var s = new StringWriter();
            attendee.WriteIcs(IcsWriter.Create(s));

            return new Attendee(new ContentLine(s.ToString()));
        }
    }
}
