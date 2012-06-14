using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExchangeRates.Context;
using ExchangeRates.Data;
using ExchangeRates.Models;
using ExchangeRates.Helpers;

namespace ExchangeRates.Repositories
{
    /// <summary>
    /// A concrete class that implements IExchangeRateRepository
    /// This Repository retrieve/update data to SQL Server 2008 Database 
    /// </summary>
    public class ExchangeRateRepository : IExchangeRateRepository
    {       
        /// <summary>
        /// Get Exchange Rate data for Date range [from, to]
        /// </summary>
        /// <param name="currency">requested Currency</param>
        /// <param name="from">start date</param>
        /// <param name="to">end date</param>
        /// <returns></returns>
        public IEnumerable<Models.ExchangeRate> GetExchangeRates(string currency, DateTime from, DateTime to)
        {
            // Create ExchangeRate database context
            using (ExchangeRateContext context = new ExchangeRateContext())
            {
                // Get data of given currency from start date to end date
                // and order by Date
                var result = context.ExchangeRates
                    .Where(er => er.Date >= from && er.Date <= to && er.Currency == currency)
                    .OrderBy(er => er.Date)
                    .ToList();

                Dictionary<DateTime, dynamic> rates = null;

                // If there are data returned
                if (result.Count() > 0)
                {
                    // Get all the dates between start date and end date
                    DateTime[] dateArray = DateTimeHelper.GetDateTimeArray(from, to);

                    // Calculate the missing dates from the data read from database
                    var existDate = result.Select(er => er.Date);
                    var nonExistDates = dateArray.Where(d => !existDate.Contains(d));

                    // Queue a request for pulling data of missing to DataBackgroundWorker
                    DataBackgroundWorker.Instance.QueueRequest(new DataRequest
                    {
                        ListOfDays = nonExistDates,                        
                    });

                    // Get data of missing dates from remoteService
                    rates = ExchangeRateClient.ReadFromRemoteService(nonExistDates);
                }
                // If no data returned at all
                else
                {
                    // get all data from remote service
                    rates = ExchangeRateClient.ReadFromRemoteService(from, to);

                    // Queue a request for pulling data from Start date to End date to DataBackgroundWorker
                    DataBackgroundWorker.Instance.QueueRequest(new DataRequest
                    {
                        ListOfDays = DateTimeHelper.GetDateTimeArray(from, to)                        
                    });
                }

                // Parse the result and add them into result collection
                // Rate of non-exist data is set with -1 
                // and the Exchange Rate object will be eliminated from the final result
                result.AddRange(rates.Select(kp =>
                {
                    return new ExchangeRate
                    {
                        Date = kp.Key,
                        Currency = currency,
                        Rate = kp.Value != null ? kp.Value["rates"][currency] : -1
                    };
                }).Where(ar => ar.Rate != -1));

                // Sort by Date and return the result
                return result.OrderBy(er => er.Date);
            }
        }

    }
}