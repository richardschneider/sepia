using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    class TimeRangeExample
    {
        #region Credit Card Check
        class CreditCard
        {
            TimeRange ValidityPeriod { get; set; }

            void CheckValidityAt(DateTimeOffset usedOn)
            {
                if (!ValidityPeriod.Contains(usedOn))
                    throw new Exception("Invalid credit card.");
            }
        }
        #endregion
        
        void Snippets()
        {
            #region Dividing
            var now = new DateTimeOffset(2013, 8, 13, 13, 0, 0, TimeSpan.FromHours(12));
            var availableTime = new TimeRange(now, now.AddHours(1));
            var freeSlots = availableTime.Divide(TimeSpan.FromMinutes(15));
            foreach (var freeSlot in freeSlots)
            {
                Console.WriteLine(freeSlot);
            }

            // Produces 4 15 minute time ranges.
            //
            // 13 Aug 2013 1:00:00 p.m. +12:00 for 00:15:00
            // 13 Aug 2013 1:15:00 p.m. +12:00 for 00:15:00
            // 13 Aug 2013 1:30:00 p.m. +12:00 for 00:15:00
            // 13 Aug 2013 1:45:00 p.m. +12:00 for 00:15:00
            #endregion

        }

        void Snippets2()
        {
            #region SubtractCollection
            var nz = TimeSpan.FromHours(12);
            var availability =
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0, nz), TimeSpan.FromHours(3)); // 0900 - 1200
            var busy = new TimeRange[]
            {
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0,  nz), TimeSpan.FromMinutes(30)), // 0900 - 0930
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 9, 0, 0,  nz), TimeSpan.FromHours(1)), // 0900 - 1000
                 new TimeRange(new DateTimeOffset(2013, 8, 13, 11, 15, 0,  nz), TimeSpan.FromMinutes(15)), // 1115 - 1130
            };
            var free = availability.Subtract(busy);
            foreach (var t in free)
                Console.WriteLine(t);

            // Produces the following free time
            // 13 Aug 2013 10:00:00 a.m. +12:00 for 01:15:00
            // 13 Aug 2013 11:30:00 a.m. +12:00 for 00:30:00
            #endregion
        }

        public void Scheduling()
        {
            #region Scheduling
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
            Console.WriteLine(when);

            // Produces: 13 Aug 2013 2:00:00 p.m. +12:00 for 02:00:00
            #endregion
        }
    }
}
