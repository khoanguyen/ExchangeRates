using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using ExchangeRates.Context;
using ExchangeRates.Helpers;
using ExchangeRates.Models;
using System.Diagnostics;
using System.Web.Configuration;
using System.Transactions;

namespace ExchangeRates.Data
{
    /// <summary>
    /// Background Worker handles pulling data from RemoteService and save it to database
    /// 
    /// The role of this worker is to avoid multiple Insert operation with the same data into database
    /// which will result in unexpected error due to duplicated primary key insert
    /// 
    /// There is only one worker and only one request is proceeded at a time.
    /// </summary>
    public class DataBackgroundWorker
    {
        // Synchronization lock for accessing the queue
        private object _lock = new object();
        
        // Worker thread which proceed the queued DataRequest
        private Thread _workerThread;

        // DataRequest queue
        private Queue<DataRequest> _queue;

        private IEnumerable<string> _trackedCurrencies;

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static DataBackgroundWorker Instance { get; private set; }

        /// <summary>
        /// Static constructor
        /// </summary>
        static DataBackgroundWorker()
        {
            // Create singleton instance and automatically start work thread for the first time this class is accessed
            Instance = new DataBackgroundWorker();
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private DataBackgroundWorker()
        {
            // Create a queue
            _queue = new Queue<DataRequest>();

            // Create worker thread
            _workerThread = new Thread(new ThreadStart(ProcessQueue));

            // Start worker thread
            _workerThread.Start();

            // Get Tracked currencies
            _trackedCurrencies = WebConfigurationManager.AppSettings["TrackedCurrencies"]
                .Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Queue a DataRequest for processing
        /// </summary>
        /// <param name="request"></param>
        public void QueueRequest(DataRequest request)
        {            
            // Gain synchronization lock
            lock (_lock)
            {
                // Enqueue Request
                _queue.Enqueue(request);

                // Notify worker thread
                Monitor.Pulse(_lock);
            }
        }

        /// <summary>
        /// Run a inifinite Loop which dequeue the queued Requests and proceed them
        /// </summary>
        private void ProcessQueue()
        {
            while (true)
            {
                // Retrieve a request from the queue
                DataRequest request = null;
                lock (_lock)
                {
                    // If the queu is empty then put Worker Thread to Wait mode 
                    // until there is a notification from Monitor.Pulse call
                    if (_queue.Count == 0) Monitor.Wait(_lock);

                    // Dequeue a request
                    request = _queue.Dequeue();
                }
                if (request != null)
                {
                    try
                    {
                        // Proceed request
                        ProcessRequest(request);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Proceed DataRequest 
        /// </summary>
        /// <param name="request"></param>
        private void ProcessRequest(DataRequest request)
        {
            // Create DB context
            using (ExchangeRateContext context = new ExchangeRateContext())
            {               
                // Load data from remote service
                dynamic data = ExchangeRateClient.ReadFromRemoteService(request.ListOfDays);

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {

                    // Insert all tracked currencies data into the context
                    foreach (string currency in _trackedCurrencies)
                        UpdateDataForCurrency(context, currency, request, data);                    

                    // Save context
                    context.SaveChanges();

                    // Complete the Transaction
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// Check and Insert data for the given currency into database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currency"></param>
        /// <param name="request"></param>
        /// <param name="data"></param>
        private void UpdateDataForCurrency(ExchangeRateContext context, string currency, DataRequest request, dynamic data)
        {
            // Get from time and to time and strip their time parts
            var dates = request.ListOfDays.ToList();

            // Retrieve the ExchangeRate data from database
            // Then filter with requested currency and request time 
            var rates = context.ExchangeRates
                               .Where(er => er.Currency == currency && dates.Contains(er.Date))
                               .Select(er => er.Date)
                               .Distinct();

            // Remove days that already has data of requested currency in the database from the date array
            foreach (var rate in rates) dates.Remove(rate.Date);

            // Get data for days that don't have data in the database
            foreach (var dateTime in dates)
            {
                // Insert data into the context
                InsertExchangeRate(context, dateTime, currency, data);
            }
        }

        /// <summary>
        /// Insert a single EnchangeRate record to the database context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="date"></param>
        /// <param name="currency"></param>
        /// <param name="data"></param>
        private void InsertExchangeRate(ExchangeRateContext context, DateTime date, string currency, dynamic data)
        {
            try
            {
                if (data[date] != null && data[date]["rates"].ContainsKey(currency))
                {
                    // Create new Exchange Rate
                    var newRate = new ExchangeRate()
                    {
                        Currency = currency,
                        Date = date,
                        Rate = data[date]["rates"][currency]
                    };

                    // Add the new ExchangeRate into the context
                    context.ExchangeRates.Add(newRate);
                }
            }
            catch (Exception ex)
            {
                // Do nothing fail data will be retrieve later
            }
        }
    }
}