using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="TimeRange"/>.
    /// </summary>
    [TestClass]
    public class TimeRangeTest
    {
        /// <summary>
        ///   The end time must be greater than the start time.
        /// </summary>
        [TestMethod]
        public void InvalidEndTime()
        {
            var now = DateTimeOffset.Now;
            var eot = DateTimeOffset.MaxValue;

            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(now, TimeSpan.FromTicks(0)));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(now, TimeSpan.FromTicks(-1)));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, TimeSpan.FromTicks(+1)));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, TimeSpan.FromTicks(0)));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, TimeSpan.FromTicks(-1)));

            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(now, now));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(now, now.AddTicks(-1)));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(now, (DateTimeOffset?) now));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(now, (DateTimeOffset?) now.AddTicks(-1)));

            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, eot));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, eot.AddTicks(-1)));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, (DateTimeOffset?)eot));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, (DateTimeOffset?)eot.AddTicks(-1)));
            ExceptionAssert.Throws<ArgumentException>(() => new TimeRange(eot, eot.AddTicks(1)));

        }

        /// <summary>
        ///   Inclusive start date and exclusive end date.
        /// </summary>
        [TestMethod]
        public void Containment()
        {
            var now = DateTimeOffset.Now;
            var start = now - TimeSpan.FromSeconds(10);
            var end = now + TimeSpan.FromSeconds(10);
            var tick = TimeSpan.FromTicks(1);
            var range = new TimeRange(start, end);

            Assert.IsFalse(range.Contains(start - tick));
            Assert.IsTrue(range.Contains(start));
            Assert.IsTrue(range.Contains(start + tick));

            Assert.IsTrue(range.Contains(end - tick));
            Assert.IsFalse(range.Contains(end));
            Assert.IsFalse(range.Contains(end + tick));
        }

        /// <summary>
        ///   End of time is special.
        /// </summary>
        [TestMethod]
        public void ContainmentEndofTime()
        {
            var now = DateTimeOffset.Now;
            var eot = DateTimeOffset.MaxValue;
            var start = now - TimeSpan.FromSeconds(10);
            var tick = TimeSpan.FromTicks(1);

            var range = new TimeRange(start);
            Assert.IsFalse(range.Contains(start - tick));
            Assert.IsTrue(range.Contains(start));
            Assert.IsTrue(range.Contains(start + tick));
            Assert.IsTrue(range.Contains(eot - tick));
            Assert.IsTrue(range.Contains(eot));
        }

        /// <summary>
        ///   Duration is simply <c>end - start</c>.
        /// </summary>
        [TestMethod]
        public void Duration()
        {
            var now = DateTimeOffset.Now;
            Assert.AreEqual(TimeSpan.FromHours(1), new TimeRange(now, now + TimeSpan.FromHours(1)).Duration);
            Assert.IsTrue(TimeSpan.FromHours(1) < new TimeRange(now).Duration);
        }

        [TestMethod]
        public void Dividing()
        {
            var now = new DateTimeOffset(2013, 8, 13, 13, 0, 0, TimeSpan.FromHours(12));
            var availableTime = new TimeRange(now, now.AddHours(1));
            var freeSlots = availableTime
                .Divide(TimeSpan.FromMinutes(15))
                .ToArray();
            Assert.AreEqual(4, freeSlots.Length);
            Assert.IsTrue(freeSlots.All(s => s.Duration == TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        public void DividingOnlyOne()
        {
            var now = new DateTimeOffset(2013, 8, 13, 13, 0, 0, TimeSpan.FromHours(12));
            var availableTime = new TimeRange(now, now.AddMinutes(16));
            var freeSlots = availableTime
                .Divide(TimeSpan.FromMinutes(15))
                .ToArray();
            Assert.AreEqual(1, freeSlots.Length);
            Assert.IsTrue(freeSlots.All(s => s.Duration == TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        public void DividingNone()
        {
            var now = new DateTimeOffset(2013, 8, 13, 13, 0, 0, TimeSpan.FromHours(12));
            var availableTime = new TimeRange(now, now.AddMinutes(14));
            var freeSlots = availableTime
                .Divide(TimeSpan.FromMinutes(15))
                .ToArray();
            Assert.AreEqual(0, freeSlots.Length);
        }

        [TestMethod]
        public void DividingEndOfTime()
        {
            var now = DateTimeOffset.MaxValue - TimeSpan.FromHours(1);
            var availableTime = new TimeRange(now, now.AddHours(1));
            var freeSlots = availableTime
                .Divide(TimeSpan.FromMinutes(15))
                .ToArray();
            Assert.AreEqual(4, freeSlots.Length);
            Assert.IsTrue(freeSlots.All(s => s.Duration == TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        public void DividingUneven()
        {
            var now = new DateTimeOffset(2013, 8, 13, 13, 0, 0, TimeSpan.FromHours(12));
            var availableTime = new TimeRange(now, now.AddMinutes(59));
            var freeSlots = availableTime
                .Divide(TimeSpan.FromMinutes(15))
                .ToArray();
            Assert.AreEqual(3, freeSlots.Length);
            Assert.IsTrue(freeSlots.All(s => s.Duration == TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        public void DividingEndOfTimeUneven()
        {
            var now = DateTimeOffset.MaxValue - TimeSpan.FromHours(1);
            var availableTime = new TimeRange(now, now.AddMinutes(59));
            var freeSlots = availableTime
                .Divide(TimeSpan.FromMinutes(15))
                .ToArray();
            Assert.AreEqual(3, freeSlots.Length);
            Assert.IsTrue(freeSlots.All(s => s.Duration == TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        public void Subtracting()
        {
            var now = new DateTimeOffset(2013, 8, 13, 13, 0, 0, TimeSpan.FromHours(12));
            var tick = new TimeSpan(1);
            var time = new TimeRange(now, now.AddHours(1));
            var before = new TimeRange(now - TimeSpan.FromHours(1), time.StartsOn);
            var after = new TimeRange(time.EndsOn, TimeSpan.FromHours(1));
            var mid = new TimeRange(now.AddMinutes(15), TimeSpan.FromMinutes(30));

            // No intersection
            Assert.AreEqual(1, time.Subtract(before).Count());
            Assert.AreEqual(time, time.Subtract(before).First());
            Assert.AreEqual(1, time.Subtract(after).Count());
            Assert.AreEqual(time, time.Subtract(after).First());

            // Identity
            Assert.AreEqual(0, time.Subtract(time).Count());

            // Subsumes
            Assert.AreEqual(0, time.Subtract(new TimeRange(before.StartsOn, after.EndsOn)).Count());

            // Middle
            Assert.AreEqual(2, time.Subtract(mid).Count());
            Assert.IsTrue(time.Subtract(mid).Any(t => t.StartsOn == now && t.Duration == TimeSpan.FromMinutes(15)));
            Assert.IsTrue(time.Subtract(mid).Any(t => t.StartsOn == now.AddMinutes(45) && t.Duration == TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        public void SubtractingAgain()
        {
            var nz = TimeSpan.FromHours(12);
            var duration = TimeSpan.FromMinutes(1);
            var a = new DateTimeOffset(2013, 8, 13, 9, 0, 0, nz);
            var b = a + duration;
            var c = b + duration;
            var d = c + duration;
            var e = d + duration;
            var f = e + duration;
            var g = f + duration;
            var h = g + duration;
            var availability = new TimeRange(c, f);

            var free = availability.Subtract(new TimeRange(a, b)).ToArray();
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(availability, free[0]);

            free = availability.Subtract(new TimeRange(b, c)).ToArray();
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(availability, free[0]);

            free = availability.Subtract(new TimeRange(f, g)).ToArray();
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(availability, free[0]);

            free = availability.Subtract(new TimeRange(g, h)).ToArray();
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(availability, free[0]);

            free = availability.Subtract(new TimeRange(b, g)).ToArray();
            Assert.AreEqual(0, free.Length);

            free = availability.Subtract(new TimeRange(d, e)).ToArray();
            Assert.AreEqual(2, free.Length);
            Assert.AreEqual(new TimeRange(c, d), free[0]);
            Assert.AreEqual(new TimeRange(e, f), free[1]);

            free = availability.Subtract(new TimeRange(b, d)).ToArray(); // fails
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(new TimeRange(d, f), free[0]);

            free = availability.Subtract(new TimeRange(e, g)).ToArray(); 
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(new TimeRange(c, e), free[0]);

            free = availability.Subtract(availability).ToArray();
            Assert.AreEqual(0, free.Length);

            free = availability.Subtract(new TimeRange(c, d)).ToArray();
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(new TimeRange(d, f), free[0]);

            free = availability.Subtract(new TimeRange(e, f)).ToArray();
            Assert.AreEqual(1, free.Length);
            Assert.AreEqual(new TimeRange(c, e), free[0]);
        }

        [TestMethod]
        public void Scheduling()
        {
            var nz = TimeSpan.FromHours(12);
            var availability = new TimeRange[]
            {
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0,  nz), TimeSpan.FromHours(3)), // 0900 - 1200
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 14, 0, 0,  nz), TimeSpan.FromHours(3)), // 1400 - 1700
            };
            var busy = new TimeRange[]
            {
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0,  nz), TimeSpan.FromMinutes(30)), // 0900 - 0930
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0,  nz), TimeSpan.FromHours(1)), // 0900 - 1000
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 11, 0, 0,  nz), TimeSpan.FromHours(1)), // 1100 - 1200
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 11, 0, 0,  nz), TimeSpan.FromHours(2)), // 1100 - 1300
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 16, 0, 0,  nz), TimeSpan.FromMinutes(30)), // 1600 - 1630
            };

            // Get the free times and then find a 2 hour slot.
            var when = availability
                .SelectMany(a => a.Subtract(busy))
                .SelectMany(a => a.Divide(TimeSpan.FromHours(2)))
                .First();
            // Produces: 13 Aug 2013 2:00:00 p.m. +12:00 for 02:00:00
            //Console.WriteLine(when);
            Assert.AreEqual(new TimeRange(new DateTimeOffset(2013, 8, 13, 14, 0, 0, nz), TimeSpan.FromHours(2)), when);
        }

        [TestMethod]
        public void ParsingIso8061()
        {
            var nz = TimeSpan.FromHours(12);
            var utc = TimeSpan.Zero;
            var expectUtc = new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0,  utc), TimeSpan.FromMinutes(30));
            var expectNz = new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0,  nz), TimeSpan.FromMinutes(30));

            Assert.AreEqual(expectUtc, TimeRange.ParseIso8061("20130813T090000Z/20130813T093000Z"));
            Assert.AreEqual(expectNz, TimeRange.ParseIso8061("20130813T090000+1200/20130813T093000+1200"));
            Assert.AreEqual(expectUtc, TimeRange.ParseIso8061("20130813T090000Z/PT30M"));
            Assert.AreEqual(expectNz, TimeRange.ParseIso8061("20130813T090000+1200/PT30M"));

            Assert.AreEqual(expectUtc, TimeRange.ParseIso8061("2013-08-13T09:00:00Z/2013-08-13T09:30:00Z"));
            Assert.AreEqual(expectNz, TimeRange.ParseIso8061("2013-08-13T09:00:00+1200/2013-08-13T09:30:00+1200"));
            Assert.AreEqual(expectUtc, TimeRange.ParseIso8061("2013-08-13T09:00:00Z/PT30M"));
            Assert.AreEqual(expectNz, TimeRange.ParseIso8061("2013-08-13T09:00:00+1200/PT30M"));
        }

        [TestMethod]
        public void Partials()
        {
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2014-01-01T00:00:00"), TimeRange.FromPartial("2013").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-02-01T00:00:00"), TimeRange.FromPartial("2013-01").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-02T00:00:00"), TimeRange.FromPartial("2013-01-01").EndsOn);

            // Assumed local time
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T13:00:00"), TimeRange.FromPartial("2013-01-01T12").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:31:00"), TimeRange.FromPartial("2013-01-01T12:30").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11"), TimeRange.FromPartial("2013-01-01T12:30:10").EndsOn);

            // Specified local time
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T13:00:00+13:00"), TimeRange.FromPartial("2013-01-01T12+13:00").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:31:00+13:00"), TimeRange.FromPartial("2013-01-01T12:30+13:00").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11+13:00"), TimeRange.FromPartial("2013-01-01T12:30:10+13:00").EndsOn);

            // UTC time
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T13:00:00Z"), TimeRange.FromPartial("2013-01-01T12Z").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:31:00Z"), TimeRange.FromPartial("2013-01-01T12:30Z").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11Z"), TimeRange.FromPartial("2013-01-01T12:30:10Z").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T13:00:00Z"), TimeRange.FromPartial("2013-01-01T12+00:00").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:31:00Z"), TimeRange.FromPartial("2013-01-01T12:30+00:00").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11Z"), TimeRange.FromPartial("2013-01-01T12:30:10+00:00").EndsOn);

            // Milliseconds are ignored.
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11"), TimeRange.FromPartial("2013-01-01T12:30:10.0").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11"), TimeRange.FromPartial("2013-01-01T12:30:10.1").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11Z"), TimeRange.FromPartial("2013-01-01T12:30:10.11Z").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11+13:00"), TimeRange.FromPartial("2013-01-01T12:30:10.1111111111111+13:00").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11"), TimeRange.FromPartial("2013-01-01T12:30:10.9").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11Z"), TimeRange.FromPartial("2013-01-01T12:30:10.999Z").EndsOn);
            Assert.AreEqual(XmlConvert.ToDateTimeOffset("2013-01-01T12:30:11+13:00"), TimeRange.FromPartial("2013-01-01T12:30:10.999999+13:00").EndsOn);
        }
    }
}
