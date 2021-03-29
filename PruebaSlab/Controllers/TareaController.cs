using PruebaSlab.Models.DB;
using PruebaSlab.Models.Tarea;
using PruebaSlab.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PruebaSlab.Controllers
{
    [Authorize]
    [RoutePrefix("api/tarea")]
    public class TareaController : ApiController
    {
        SlabEntities DB = null;
        TareaTransaction TT = new TareaTransaction();
        [HttpGet]
        [Route("GetObj")]
        public IHttpActionResult Index()
        {
            return Ok(TT.Index(null));
        }

        [HttpGet]
        [Route("List")]
        public IHttpActionResult List()
        {
            return Ok(TT.List(null));
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult COUProyect(TareaEdit model)
        {
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
                Proyecto P = DB.Proyecto.Where(x => x.Id == model.Id_Proyecto).FirstOrDefault();
                if (R != null && P != null)
                {
                    if (model.Id == 0)
                    {
                        return Ok(TT.Creacion(model));
                    }
                    else
                    {
                        return Ok(TT.Actualizacion(model));
                    }
                }
                else
                {
                    return BadRequest("El usuario logueado no tiene permisos para realizar la acción o la informacion no esta completa.");
                }
            }
            else 
            {
                return BadRequest("No se puede validar la autenticacion del usuario.");
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
                    return Ok(TT.ChangeEstado(id));
                }
                else
                {
                    return BadRequest("El usuario logueado no tiene permisos para realizar la acción.");
                }
            }
            else 
            {
                return BadRequest("No se puede verificar la autenticacion del usuario.");
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            return Ok(TT.Delete(id));
        }
    }
}