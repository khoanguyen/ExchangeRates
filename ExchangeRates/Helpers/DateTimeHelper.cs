using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExchangeRates.Helpers
{
    /// <summary>
    /// Helper methods for DateTime manipulation
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Return a new DateTime object with date part of the given datetime 
        /// but without the time part (hour, minute, second ... are 0)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime StripTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// Return array of dates from FromDate to ToDate including FromDate and ToDate        
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static DateTime[] GetDateTimeArray(DateTime from, DateTime to)
        {
            // Strip time parts
            from = StripTime(from);
            to = StripTime(to);

            // Return empty array if to < from
            if (to < from) return new DateTime[0];

            // Calculate difference between 2 dates
            var dif = to.Subtract(from);

            // Initialize and add date to result array
            DateTime[] result = new DateTime[dif.Days + 1];
            for (int i = 0; i <= dif.Days; i++)
            {
                result[i] = from;
                from = from.AddDays(1);
            }

            // Return result
            return result;
        }
    }
}