using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Models
{
    /// <summary>
    /// ExchangeRate entity
    /// </summary>
    public class ExchangeRate
    {
        /// <summary>
        /// Date of ExchangeRate, member of composite key
        /// </summary>
        [Key, Column(Order = 0)]
        public DateTime Date { get; set; }

        /// <summary>
        /// Currency of ExchangeRate, member of composite key
        /// </summary>
        [Key, Column(Order = 1)]
        public string Currency { get; set; }

        /// <summary>
        /// ExchangeRate value
        /// </summary>
        public decimal Rate { get; set; }
    }
}