using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExchangeRates.Data
{
    /// <summary>
    /// DataRequest Object which is used to enqueue a request for pulling data from Remote service to database
    /// </summary>
    public class DataRequest
    {
        /// <summary>
        /// List of dates that need to be pulled
        /// </summary>
        public IEnumerable<DateTime> ListOfDays { get; set; }    
    }
}