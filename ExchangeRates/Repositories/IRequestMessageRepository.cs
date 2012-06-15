using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExchangeRates.Models;

namespace ExchangeRates.Repositories
{
    /// <summary>
    /// Request Message repository provides business logic on RequestMessages table
    ///
    /// RequestMessages table is where the DataRequest messages is stored for pulling datafrom the remote service.
    /// This repository acts like a queue
    /// </summary>
    public interface IRequestMessageRepository
    {
        /// <summary>
        /// Enqueue a Request with given dateList
        /// </summary>
        /// <param name="dateList"></param>
        void EnqueueRequest(DateTime[] dateList);

        /// <summary>
        /// Dequeue a Request with a unique token and return its DateTimeList[]
        /// </summary>
        /// <param name="uniqueToken"></param>
        /// <returns></returns>
        DateTime[] DequeueRequest(string uniqueToken);
    }
}
