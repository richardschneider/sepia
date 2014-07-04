using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring.Serialization
{
    [TestClass]
    public class IcsReaderTest
    {
        /// <summary>
        ///   A null ContentLine is returned when no more data is present.
        /// </summary>
        [TestMethod]
        public void Eof()
        {
            var ics = "BEGIN:VCALENDAR\r\nEND:VCALENDAR";
            var reader = IcsReader.Create(new StringReader(ics));
            ContentLine line;

            line = reader.ReadContentLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("BEGIN", line.Name);
            Assert.AreEqual("VCALENDAR", line.Value);

            line = reader.ReadContentLine();
            Assert.IsNotNull(line);
            Assert.AreEqual("END", line.Name);
            Assert.AreEqual("VCALENDAR", line.Value);

            line = reader.ReadContentLine();
            Assert.IsNull(line);
        }

        /// <summary>
        ///   A content line can minimally consists of a name and a value.
        /// </summary>
        [TestMethod]
        public void Simple()
        {
            var line = new ContentLine("DESCRIPTION:This is a long description that exists on a long line.");
            Assert.AreEqual("DESCRIPTION", line.Name);
            Assert.AreEqual("This is a long description that exists on a long line.", line.Value);
        }

        /// <summary>
        ///   An RRULE appears to have parameters but they are really part of the value.
        /// </summary>
        [TestMethod]
        public void RRule()
        {
            var line = new ContentLine("RRULE:FREQ=MONTHLY;BYDAY=MO,TU,WE,TH,FR;BYSETPOS=-2");
            Assert.AreEqual("RRULE", line.Name);
            Assert.AreEqual("FREQ=MONTHLY;BYDAY=MO,TU,WE,TH,FR;BYSETPOS=-2", line.Value);
        }

        /// <summary>
        ///   A content line can be folded onto the next line.
        /// </summary>
        [TestMethod]
        public void Folded()
        {
            var line = new ContentLine("DESCRIPTION:This is a lo\r\n ng description\r\n\t that exists on a long line.");
            Assert.AreEqual("DESCRIPTION", line.Name);
            Assert.AreEqual("This is a long description that exists on a long line.", line.Value);
        }

        /// <summary>
        ///   A content line can contain multiple values separated by a comma.
        /// </summary>
        [TestMethod]
        public void MultiValued()
        {
            var line = new ContentLine(@"x:alpha,beta,omega");
            Assert.AreEqual("alpha,beta,omega", line.Value);
            Assert.AreEqual(3, line.Values.Length);
            Assert.AreEqual("alpha", line.Values[0]);
            Assert.AreEqual("beta", line.Values[1]);
            Assert.AreEqual("omega", line.Values[2]);
        }

        /// <summary>
        ///   The comma can be escaped with a back-slash.
        /// </summary>
        [TestMethod]
        public void CommaEscapedValue()
        {
            var line = new ContentLine(@"x:alpha\,beta\,omega");
            Assert.AreEqual("alpha,beta,omega", line.Value);
            Assert.AreEqual(1, line.Values.Length);
            Assert.AreEqual("alpha,beta,omega", line.Values[0]);
        }

        /// <summary>
        ///   A content line can contain parameters.
        /// </summary>
        [TestMethod]
        public void Parameters()
        {
            var line = new ContentLine("DESCRIPTION;foo=1;bar=2:This is a long description that exists on a long line.");
            Assert.AreEqual("DESCRIPTION", line.Name);
            Assert.AreEqual("This is a long description that exists on a long line.", line.Value);
            Assert.AreEqual("1", line.Parameters["foo"]);
            Assert.AreEqual("2", line.Parameters["bar"]);
        }

        /// <summary>
        ///   A content line can contain a parameter with multiple values.
        /// </summary>
        [TestMethod]
        public void MultivaluedParameter()
        {
            var line = new ContentLine("DESCRIPTION;foo=1;bar=2,3:This is a long description that exists on a long line.");
            Assert.AreEqual("DESCRIPTION", line.Name);
            Assert.AreEqual("This is a long description that exists on a long line.", line.Value);
            Assert.AreEqual("1", line.Parameters["foo"]);
            Assert.AreEqual(2, line.Parameters.GetValues("bar").Length, "expected 2 values");
            Assert.AreEqual("2", line.Parameters.GetValues("bar")[0]);
            Assert.AreEqual("3", line.Parameters.GetValues("bar")[1]);
        }

        /// <summary>
        ///   A content line can contain a parameter with multiple quoted values.
        /// </summary>
        [TestMethod]
        public void MultivaluedQuotedParameter()
        {
            var line = new ContentLine("DESCRIPTION;foo=\":alpha:\",\":beta:\":This is a long description that exists on a long line.");
            Assert.AreEqual("DESCRIPTION", line.Name);
            Assert.AreEqual("This is a long description that exists on a long line.", line.Value);
            Assert.AreEqual(2, line.Parameters.GetValues("foo").Length, "expected 2 values");
            Assert.AreEqual(":alpha:", line.Parameters.GetValues("foo")[0]);
            Assert.AreEqual(":beta:", line.Parameters.GetValues("foo")[1]);
        }

        /// <summary>
        ///   A value can be escaped.
        /// </summary>
        [TestMethod]
        public void EscapedValue()
        {
            var line = new ContentLine(@"DESCRIPTION:Project XYZ Final Review\nConference Room - 3B\nCome Prepared.");
            Assert.AreEqual("Project XYZ Final Review\r\nConference Room - 3B\r\nCome Prepared.", line.Value);
        }

        /// <summary>
        ///   A quoted parameter value can contain special characters.
        /// </summary>
        [TestMethod]
        public void QuotedParameterValue()
        {
            var line = new ContentLine("DESCRIPTION;ALTREP=\"cid:part1.0001@example.org\":The Fall'98 Wild Wizards Conference - - Las Vegas\\, NV\\, USA");
            Assert.AreEqual("cid:part1.0001@example.org", line.Parameters["ALTREP"]);
            Assert.AreEqual("The Fall'98 Wild Wizards Conference - - Las Vegas, NV, USA", line.Value);
        }

        /// <summary>
        ///   Components can can sub-components.
        /// </summary>
        [TestMethod]
        public void Components()
        {
#if false // TODO
            var reader = IcsReader.Create(new StringReader(Properties.Resources.Sample3));
            var component = reader.ReadComponent();
            Assert.IsInstanceOfType(component, typeof(Calendar));
            Assert.AreEqual(2, component.Components.Count);
            Assert.IsInstanceOfType(component.Components[0], typeof(TimeZone));
            Assert.IsInstanceOfType(component.Components[1], typeof(Event));
            Assert.AreEqual(2, component.Components[0].Components.Count);
            Assert.IsInstanceOfType(component.Components[0].Components[0], typeof(StandardTime));
            Assert.IsInstanceOfType(component.Components[0].Components[1], typeof(DaylightSavingTime));
            Assert.AreEqual(1, component.Components[1].Components.Count);
            Assert.IsInstanceOfType(component.Components[1].Components[0], typeof(Alarm));
#endif
        }

        /// <summary>
        ///   All content lines MUST have a value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CalendarException))]
        public void MissingValue()
        {
            var content = new ContentLine("description;language=en-NZ;Colouring"); // 2nd ';' should be ':'
        }

        /// <summary>
        ///   All content lines MUST have a value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CalendarException))]
        public void MissingValue2()
        {
            var content = new ContentLine("description;language=en-NZ"); 
        }

        /// <summary>
        ///   All content lines MUST have a value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CalendarException))]
        public void MissingValue3()
        {
            var content = new ContentLine("description:");
        }

        /// <summary>
        ///   All content lines MUST have a value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CalendarException))]
        public void MissingValue4()
        {
            var content = new ContentLine("description");
        }
    }
}
