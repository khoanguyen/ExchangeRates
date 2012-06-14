using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExchangeRates.Models;
using ExchangeRates.Data;
using ExchangeRates.Repositories;

namespace ExchangeRates.Controllers
{
    public class HomeController : Controller
    {
        private IExchangeRateRepository _exchangeRateRepository;
        
        public HomeController(IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        /// <summary>
        /// Index 
        /// GET: /
        /// GET: /Home/
        /// GET: /Home/Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {        
            // Set Tracked currencies for rendering in View
            ViewData["TrackedCurrencies"] = (IEnumerable<string>)this.HttpContext.Application["TrackedCurrencies"];
            
            // Render view
            return View();
        }

        /// <summary>
        /// POST Index
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult Index(ExchangeInput input)
        {
            // Validate Input
            ValidateInput(input);

            // Set Tracked currencies for rendering in View
            ViewData["TrackedCurrencies"] = (IEnumerable<string>)this.HttpContext.Application["TrackedCurrencies"];
            
            // Render Index view with input
            return View("Index", input);
        }

        /// <summary>
        /// Render Chart
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ActionResult Chart(ExchangeInput input)
        {
            // Initialize empty data
            IEnumerable<ExchangeRate> data = new ExchangeRate[0];

            // Get data from repository and intialize necessary param for Rendering
            if (input != null)
            {
                // Get data from repository
                data = _exchangeRateRepository.GetExchangeRates(input.TargetCurrency, input.StartDate, input.EndDate);

                // Set param for rendering
                ViewBag.Currency = input.TargetCurrency;
                ViewBag.From = input.StartDate;
                ViewBag.To = input.EndDate;
            }

            // Render view
            return View(data);
        }

        /// <summary>
        /// Explicitly validate Exhcange Input
        /// </summary>
        /// <param name="input"></param>
        private void ValidateInput(ExchangeInput input)
        {
            // If no start date selected
            if (input.StartDate == null)            
                ModelState.AddModelError("StartDate", "Please select a Start date");

            // If no end date selected
            if (input.EndDate == null)
                ModelState.AddModelError("EndDate", "Please select a End date");
            
            if (input.StartDate != null && input.EndDate != null)
            {
                // Max expected end date
                var expectedEndDate = input.StartDate.AddMonths(2);

                // Invalid date range
                if (input.EndDate < input.StartDate)
                    ModelState.AddModelError("DateRange", "Invalid Range of Start date and End date. End date should not earlier than Start date");
                else if (input.EndDate > expectedEndDate)
                    ModelState.AddModelError("DateRange", "Invalid Range of Start date and End date. Range should not exceed 2 months");

                // Invalid start date
                if (input.StartDate < new DateTime(1999, 1, 1))
                    ModelState.AddModelError("InvalidStartDate", "StartDate must be later than 1/1/1999");
                else if (input.StartDate > DateTime.Now)
                    ModelState.AddModelError("InvalidStartDate", "No data found");
            }
        }
    }
}
