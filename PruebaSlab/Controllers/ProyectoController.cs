using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PruebaSlab.Controllers
{
    [Authorize]
    [RoutePrefix("api/proyecto")]
    public class ProyectoController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index()
        {
            return null;
        }
    }
}