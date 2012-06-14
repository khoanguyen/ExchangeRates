using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ExchangeRates.Models;

namespace ExchangeRates.Context
{
    /// <summary>
    /// ExchangeRateDB database context
    /// </summary>
    public class ExchangeRateContext : DbContext
    {
        /// <summary>
        /// ExchangeRates DbSEt for ExchangeRates table
        /// </summary>
        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        /// <summary>
        /// Override
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            // Set precision of ExchangeRate.Rate field to 18 number with 6 number on the right side
            modelBuilder.Entity<ExchangeRate>().Property(obj => obj.Rate).HasPrecision(18, 6);
        }
    }
}