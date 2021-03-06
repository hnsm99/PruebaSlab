using PruebaSlab.Models;
using PruebaSlab.Transaction;
using PruebaSlab.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace PruebaSlab.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        TokenGenerator TG = new TokenGenerator();
        Response response = new Response();
        [HttpGet]
        [Route("echoping")]
        public IHttpActionResult EchoPing()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }


        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Index(LoginRequest login)
        {
            
            if (login==null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            login.Password = response.Encriptar(login.Password);
            return Ok(TG.GenerateTokenAwt(login));
        }
    }
}