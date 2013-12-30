using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring.Serialization
{
    [TestClass]
    public class IcsWriterTest
    {
        /// <summary>
        ///   Lines are folded after octets per line is reached.
        /// </summary>
        [TestMethod]
        public void LineFolding()
        {
            var line = new ContentLine("prop12;foo=1;bar=2:word1 word2");
            var settings = new IcsWriterSettings() { OctetsPerLine = 7 };
            var s = new StringWriter();
            using (var writer = IcsWriter.Create(s, settings))
            {
                writer.Write(line);
            }
            var ics = s.ToString().Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            Assert.AreEqual(5, ics.Length);
            Assert.AreEqual("PROP12;", ics[0]);
            Assert.AreEqual(" FOO=1;", ics[1]);
            Assert.AreEqual(" BAR=2:", ics[2]);
            Assert.AreEqual(" word1 ", ics[3]);
            Assert.AreEqual(" word2", ics[4]);
        }

        /// <summary>
        ///   A line must be folded when it greater octets per line.
        /// </summary>
        public void LineFolding2()
        {
            var line = new ContentLine("ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE:mailto:everyone@somewhere.org");
            var settings = new IcsWriterSettings();
            var s = new StringWriter();
            using (var writer = IcsWriter.Create(s, settings))
            {
                writer.Write(line);
            }
            var lines = s.ToString().Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            foreach (var l in lines)
                Assert.IsTrue(Encoding.UTF8.GetByteCount(l) <= settings.OctetsPerLine);
        }

        /// <summary>
        ///   CRLF and ',' are escaped in a value.
        /// </summary>
        [TestMethod]
        public void EscapingValue()
        {
            var line = new ContentLine();
            line.Name = "x";
            line.Value = "This is a long description\\,\r\nthat exists on two lines.";
            Assert.AreEqual(1, line.Values.Length);
            Assert.AreEqual(@"X:This is a long description\,\nthat exists on two lines.", line.ToString());
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
            Assert.AreEqual(@"X:alpha,beta,omega", line.ToString());
        }

        /// <summary>
        ///   Parameter values with ':', ';' or ',' are quoted.
        /// </summary>
        [TestMethod]
        public void QuoteParameterValue()
        {
            var line = new ContentLine();
            line.Name = "x";
            line.Value = "x";
            line.Parameters.Add("a", "alpha");
            line.Parameters.Add("b", ":beta:");
            line.Parameters.Add("c", "alpha;beta");
            line.Parameters.Add("d", "alpha,beta");
            Assert.AreEqual("X;A=alpha;B=\":beta:\";C=\"alpha;beta\";D=\"alpha,beta\":x", line.ToString());
        }

        /// <summary>
        ///   Components can be nested.
        /// </summary>
        [TestMethod]
        public void NestedComponents()
        {
            var settings = new IcsWriterSettings();
            var s = new StringWriter();
            using (var writer = IcsWriter.Create(s, settings))
            {
                writer.WriteBeginComponent("A");
                writer.WriteBeginComponent("B");
                writer.WriteEndComponent();
                writer.WriteEndComponent();
            }

            var ics = s.ToString().Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            Assert.AreEqual(4, ics.Length);
            Assert.AreEqual("BEGIN:A", ics[0]);
            Assert.AreEqual("BEGIN:B", ics[1]);
            Assert.AreEqual("END:B", ics[2]);
            Assert.AreEqual("END:A", ics[3]);
        }
        
        [TestMethod]
        public void UtcOffset()
        {
            var zero = TimeSpan.Zero;
            var one = new TimeSpan(1, 30, 0);
            var minusOne = new TimeSpan(-1, -30, 0);
            var s = new StringWriter();
            using (var writer = IcsWriter.Create(s))
            {
                writer.WriteContentAsUtcOffset("zero", zero);
                writer.WriteContentAsUtcOffset("one", one);
                writer.WriteContentAsUtcOffset("minusone", minusOne);
            }
            var ics = s.ToString().Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            Assert.AreEqual(3, ics.Length);
            Assert.AreEqual("ZERO:0000", ics[0]);
            Assert.AreEqual("ONE:+0130", ics[1]);
            Assert.AreEqual("MINUSONE:-0130", ics[2]);
        }

        /// <summary>
        ///   Must have match begin/end component.
        /// </summary>
        [TestMethod]
        public void ComponentNotClosed()
        {
            var settings = new IcsWriterSettings();
            var s = new StringWriter();
            var writer = IcsWriter.Create(s, settings);
            writer.WriteBeginComponent("A");
            writer.WriteBeginComponent("B");
            writer.WriteEndComponent();
            ExceptionAssert.Throws<CalendarException>(() => writer.Dispose());
        }

        [TestMethod]
        public void SerialiseTimeSpan()
        {
            var s = new StringWriter();
            using (var writer = IcsWriter.Create(s))
            {
                writer.WriteContent("duration", new TimeSpan(1, 2, 3, 4));
            }
            var ics = s.ToString().Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            Assert.AreEqual(1, ics.Length);
            Assert.AreEqual("DURATION:P1DT2H3M4S", ics[0]);
        }

    }
}
