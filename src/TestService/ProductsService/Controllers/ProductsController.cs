using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ProductsService.Controllers
{
    /// <summary>
    /// Controller to operate with products
    /// </summary>
    public class ProductsController : ApiController
    {

        /// <summary>
        /// Returns all available products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool IsAlive()
        {
            return true;
        }

    }
}
