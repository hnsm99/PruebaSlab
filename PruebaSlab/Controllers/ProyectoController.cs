using PruebaSlab.Transaction;
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
        ProyectoTransaction PT = new ProyectoTransaction();
        [HttpGet]
        [Route("GetObj")]
        public IHttpActionResult Index()
        {
            return Ok(PT.Index(null));
        }
    }
}