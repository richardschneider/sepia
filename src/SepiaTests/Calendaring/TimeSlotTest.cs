using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{

    [TestClass]
    public class TimeSlotTest
    {
        /// <summary>
        ///   The default <see cref="TimeSlot.Usage"/> is busy.
        /// </summary>
        [TestMethod]
        public void Defaults()
        {
            Assert.IsTrue(new TimeSlot().IsBusy);
        }

        /// <summary>
        ///   A <see cref="TimeSlot"/> is considered busy unless the <see cref="TimeSlot.Usage"/> is
        ///   equal <see cref="TimeUsage.Free"/>
        /// </summary>
        [TestMethod]
        public void IsBusy()
        {
            Assert.IsTrue(new TimeSlot { Usage = new TimeUsage { Name = "not free" } }.IsBusy);
            Assert.IsFalse(new TimeSlot { Usage = new TimeUsage { Name = "free" } }.IsBusy);

            Assert.IsFalse(new TimeSlot { Usage = new TimeUsage { Name = "not free" } }.IsFree);
            Assert.IsTrue(new TimeSlot { Usage = new TimeUsage { Name = "free" } }.IsFree);
        }

        /// <summary>
        ///   A <see cref="TimeSlot.Usage"/> can be deserialised from a <see cref="ContentLine"/>.
        /// </summary>
        [TestMethod]
        public void UsageFromContentLine()
        {
            var slot = new TimeSlot(new ContentLine("FREEBUSY;FBTYPE=BUSY:19980415T133000Z/19980415T170000Z"));
            Assert.AreEqual(TimeUsage.Busy, slot.Usage);

            slot = new TimeSlot(new ContentLine("FREEBUSY;FBTYPE=BUSY-tentative:19980415T133000Z/19980415T170000Z"));
            Assert.AreEqual(TimeUsage.Tentative, slot.Usage);

            slot = new TimeSlot(new ContentLine("FREEBUSY:19980415T133000Z/19980415T170000Z"));
            Assert.AreEqual(TimeUsage.Busy, slot.Usage);
        }

        /// <summary>
        ///   The <see cref="TimeRange.EndsOn"/> can be a specific date or a duration.
        /// </summary>
        [TestMethod]
        public void EndsOnIsDateTimeOrDuration()
        {
            var start = new DateTimeOffset(1998, 04, 15, 13, 30, 00, TimeSpan.Zero);
            var end =   new DateTimeOffset(1998, 04, 15, 17, 00, 00, TimeSpan.Zero);

            var slot = new TimeSlot(new ContentLine("FREEBUSY:19980415T133000Z/19980415T170000Z"));
            Assert.AreEqual(1, slot.TimeRanges.Count);
            Assert.AreEqual(start, slot.TimeRanges[0].StartsOn);
            Assert.AreEqual(end, slot.TimeRanges[0].EndsOn);

            slot = new TimeSlot(new ContentLine("FREEBUSY:19980415T133000Z/PT3H30M"));
            Assert.AreEqual(1, slot.TimeRanges.Count);
            Assert.AreEqual(start, slot.TimeRanges[0].StartsOn);
            Assert.AreEqual(end, slot.TimeRanges[0].EndsOn);
        }

        /// <summary>
        ///   A <see cref="TimeSlot"/> can be serialised.
        /// </summary>
        [TestMethod]
        public void Serialisation()
        {
            var start = new DateTimeOffset(1998, 04, 15, 13, 30, 00, TimeSpan.Zero);
            var end = new DateTimeOffset(1998, 04, 15, 17, 00, 00, TimeSpan.Zero);

            var slot = WriteRead("FREEBUSY;FBTYPE=FREE:19980415T133000Z/PT3H30M");
            Assert.AreEqual(TimeUsage.Free, slot.Usage);
            Assert.AreEqual(1, slot.TimeRanges.Count);
            Assert.AreEqual(start, slot.TimeRanges[0].StartsOn);
            Assert.AreEqual(end, slot.TimeRanges[0].EndsOn);
        }

        /// <summary>
        ///   A <see cref="TimeSlot"/> can contain multiple time ranges.
        /// </summary>
        [TestMethod]
        public void MultipleTimeRanges()
        {
            var slot = WriteRead("FREEBUSY;FBTYPE=FREE:19970308T160000Z/PT3H,19970308T160000Z/PT1H");
            Assert.AreEqual(TimeUsage.Free, slot.Usage);
            Assert.AreEqual(2, slot.TimeRanges.Count);
        }

        TimeSlot WriteRead(string ics)
        {
            var slot = new TimeSlot(new ContentLine(ics));
            var s = new StringWriter();
            slot.WriteIcs(IcsWriter.Create(s));

            return new TimeSlot(new ContentLine(s.ToString()));
        }


    }
}
