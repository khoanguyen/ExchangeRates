using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExchangeRates.Context;
using ExchangeRates.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.SqlClient;

namespace ExchangeRates.Repositories
{
    /// <summary>
    /// Request Message Repository
    /// 
    /// Acting as a queue to request for downloading data from the remote service
    /// </summary>
    public class RequestMessageRepository : IRequestMessageRepository
    {

        /// <summary>
        /// Enqueue a list of RequestMessage with the given date list
        /// </summary>
        /// <param name="dateList"></param>
        public void EnqueueRequest(DateTime[] dateList)
        {
            using (var context = new ExchangeRateContext())
            {
                // Create the RequestMessage
                RequestMessage message = new RequestMessage
                {
                    // Serialize the date list
                    DateList = Serialize(dateList)
                };

                // Add it into database
                context.RequestMessages.Add(message);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Dequeue the earliest unproceeded RequestMessage and return its date list
        /// 
        /// No request found will result to an empty date list
        /// </summary>
        /// <param name="uniqueToken"></param>
        /// <returns></returns>
        public DateTime[] DequeueRequest(string uniqueToken)
        {
            using (var context = new ExchangeRateContext())
            {
                // Execute stored procedure to dequeue the expected RequestMessage
                var result = context.Database.SqlQuery<RequestMessage>("exec DequeueMessage @uniqueToken",
                    new SqlParameter("@uniqueToken", uniqueToken)).ToArray();
                                
                DateTime[] dateList = null;
                
                if (result.Count() == 0) dateList = new DateTime[0];
                else
                {
                    // Get the message
                    var message = result.First();

                    // Deserialize its date list
                    dateList = Deserialize<DateTime[]>(message.DateList);

                    // Fetch all proceeded messages from the context
                    var deleted = context.RequestMessages.Where(m => m.Token != null);
                    
                    // Delete them all
                    foreach(var deletedMessage in deleted) context.RequestMessages.Remove(deletedMessage);
                    context.SaveChanges();
                }

                // Return result
                return dateList;
            }
        }

        /// <summary>
        /// Serialize the given object and return base64 string
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private string Serialize(object graph)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                // Serialize object
                serializer.Serialize(ms, graph);

                // Return base64 string
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Deserialize into an object with type T from bse64 string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="base64"></param>
        /// <returns></returns>
        private T Deserialize<T>(string base64)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                // Deserialize and Return result
                return (T)serializer.Deserialize(ms);
            }
        }
    }
}