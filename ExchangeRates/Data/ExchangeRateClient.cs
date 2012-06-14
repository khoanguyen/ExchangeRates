using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Threading;
using ExchangeRates.Helpers;

namespace ExchangeRates.Data
{
    /// <summary>
    /// Client of Exchange Rate services
    /// This client helps pulling data from remote service
    /// </summary>
    public static class ExchangeRateClient
    {
        // Base Address of the service
        private static string _serviceBase;

        // Serializer for Deserializing JSON result
        private static JavaScriptSerializer _serializer;
        
        /// <summary>
        /// Static Controller
        /// </summary>
        static ExchangeRateClient()
        {
            // Get base address from configuration
            _serviceBase = WebConfigurationManager.AppSettings["ExchangeRateServiceURL"];

            // Create a serializer
            _serializer = new JavaScriptSerializer();           
        }

        /// <summary>
        /// Get exchange rate data from given FromDate to given ToDate
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, dynamic> ReadFromRemoteService(DateTime fromDate, DateTime toDate)
        {            
            // Get all the date between From and To dates
            var dates = DateTimeHelper.GetDateTimeArray(fromDate, toDate);

            // Return the result
            return ReadFromRemoteService(dates);            
        }

        /// <summary>
        /// Get exchange rate data for given dates
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, dynamic> ReadFromRemoteService(IEnumerable<DateTime> dates)
        {            
            Dictionary<DateTime, dynamic> result = new Dictionary<DateTime, dynamic>();

            // Limits number of concurrent operations run by Parallel method calls
            // without this Parallel operations can make the server overload
            // default value of this option is -1, means unlimited, 
            // which could result to hundreds concurrent operations
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 4;

            // Use Parallel to download the data from Remote Service
            Parallel.ForEach(dates, options,
                new Action<DateTime>(d =>
                {
                    // Get data of given date and put it into result list
                    result[d] = ReadDateFromRemoteService(d);                    
                }));

            // Return result
            return result;
        }

        /// <summary>
        /// Get exchange data for a specific date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static dynamic ReadDateFromRemoteService(DateTime date)
        {
            // Download data using Web Client
            using (var webClient = new WebClient()
            {
                // Set Base Address
                BaseAddress = _serviceBase.EndsWith("/") ? _serviceBase : _serviceBase + "/"
            })
            {
                try
                {
                    // Get date token
                    var dateToken = date.ToString("yyyy-MM-dd");

                    // Download data
                    string jsonString = webClient.DownloadString("historical/" + dateToken + ".json");

                    // Deserialize data and return result
                    return _serializer.Deserialize<dynamic>(jsonString);
                }
                catch (WebException ex)
                {
                    // Some dates don't have any data at all and they result into 404, such as April 1st and April 2nd of 2012
                    // or the download operation is failed
                    // Just return null, they will be re-checked when this dates is requested again
                    return null;
                }
            }
        }

    }
}