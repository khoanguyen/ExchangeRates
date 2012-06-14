using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExchangeRates.Models
{
    /// <summary>
    /// Form model for ExchangeRate input
    /// Used by HomeController
    /// </summary>
    public class ExchangeInput
    {        
        /// <summary>
        /// Selected currency to be compared with USD
        /// </summary>
        public string TargetCurrency { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}