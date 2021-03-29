using PruebaSlab.Models.DB;
using PruebaSlab.Models.Proyecto;
using PruebaSlab.Transaction;
using PruebaSlab.Utilities;
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
        SlabEntities DB = null;
        ProyectoTransaction PT = new ProyectoTransaction();
        [HttpGet]
        [Route("GetObj")]
        public IHttpActionResult Index()
        {
            return Ok(PT.Index(null));
        }

        [HttpGet]
        [Route("List")]
        public IHttpActionResult List()
        {
            return Ok(PT.List(null));
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult COUProyect(ProyectoEdit model) 
        {
            DB = new SlabEntities();
            int Id_Usuario;
            int Id_Rol;
            TokenGenerator TG = new TokenGenerator();
            string result = TG.DecodeTokenAwt(Request.Headers.Authorization.ToString().Replace("Bearer ","")).Result;
            if (!string.IsNullOrEmpty(result))
            {
                Id_Usuario = int.Parse(result.Split('|')[0]);
                Id_Rol = int.Parse(result.Split('|')[2]);
                Rol R = DB.Rol.Where(x => x.Id == Id_Rol && x.Rol1.ToUpper().Equals("OPERARIO")).FirstOrDefault();
                Usuario Us = DB.Usuario.Where(x => x.Id == Id_Usuario).FirstOrDefault();
                if (R != null&&Us!=null)
                {
                    if (model.Id == 0)
                    {
                        model.Id_Operario = Id_Usuario;
                        return Ok(PT.Creacion(model));
                    }
                    else
                    {
                        model.Id_Operario = Id_Usuario;
                        return Ok(PT.Actualizacion(model));
                    }
                }
                else
                {
                    return BadRequest("El usuario logueado no tiene permisos para realizar la acción.");
                }
            }
            else 
            {
                return BadRequest("No se tiene vericidad en la autenticación del usuario.");
            }
        }

        [HttpPut]
        [Route("ChEstate/{id}")]
        public IHttpActionResult CEst(int id)
        {
            var respuesta = "";
            DB = new SlabEntities();
            int Id_Usuario;
            int Id_Rol;
            TokenGenerator TG = new TokenGenerator();
            string result = TG.DecodeTokenAwt(Request.Headers.Authorization.ToString().Replace("Bearer ", "")).Result;
            if (!string.IsNullOrEmpty(result))
            {
                Id_Usuario = int.Parse(result.Split('|')[0]);
                Id_Rol = int.Parse(result.Split('|')[2]);
                Rol R = DB.Rol.Where(x => x.Id == Id_Rol && x.Rol1.ToUpper().Equals("OPERARIO")).FirstOrDefault();
                if (R != null)
                {
                    return Ok(PT.ChangeEstado(id));
                }
                else
                {
                    return BadRequest("El usuario logueado no tiene permisos para realizar la acción.");
                }
            }
            else 
            {
                return BadRequest("No es posible verificar la autenticacion del usuario.");
            }
            return Ok(respuesta);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IHttpActionResult Delete(int id) 
        {
            return Ok(PT.Delete(id));
        }
    }
}