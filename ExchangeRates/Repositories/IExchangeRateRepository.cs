using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExchangeRates.Models;

namespace ExchangeRates.Repositories
{
    /// <summary>
    /// Interface of ExchangeRateRepository
    /// </summary>
    public interface IExchangeRateRepository
    {
        /// <summary>
        /// Get Exchange rate data of given currency from start date to end date
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IEnumerable<ExchangeRate> GetExchangeRates(string currency, DateTime from, DateTime to);
    }
}
