using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using ExchangeRates.Controllers;
using ExchangeRates.Repositories;

namespace ExchangeRates.DI
{
    /// <summary>
    /// Simple Controller Factory for creating HomeController with ExchangeRateRepository
    /// </summary>
    public class MyControllerFactory: IControllerFactory
    {
        /// <summary>
        /// Create HomeController with ExchangeRateRepository        
        /// Other controller name will be resulted in null
        /// In practice this should be replace with a factory which powered by Unity or Ninject or other DI Containers
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            if (controllerName == "Home")
                return new HomeController(new ExchangeRateRepository());
            return null;
        }

        /// <summary>
        /// Implement GetControllerSessionBehavior
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        /// <summary>
        /// Implement ReleaseController
        /// </summary>
        /// <param name="controller"></param>
        public void ReleaseController(IController controller)
        {
            // Do Nothing
        }
    }
}