using PruebaSlab.Models.DB;
using PruebaSlab.Models.Usuario;
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
    [RoutePrefix("api/usuario")]
    public class UsuarioController : ApiController
    {
        SlabEntities DB = null;
        UsuarioTransaction UT = new UsuarioTransaction();
        [HttpGet]
        [Route("GetObj")]
        public IHttpActionResult Index()
        {
            return Ok(UT.Index(null));
        }

        [HttpGet]
        [Route("List")]
        public IHttpActionResult List()
        {
            return Ok(UT.List(null));
        }

        [HttpGet]
        [Route("ObjChPass")]
        public IHttpActionResult ObjPass() 
        {
            return Ok(UT.IndexPass());
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult COUProyect(UsuarioEdit model)
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
                Rol R = DB.Rol.Where(x => x.Id == Id_Rol && x.Rol1.ToUpper().Equals("ADMINISTRADOR")).FirstOrDefault();
                if (R != null)
                {
                    return Ok(UT.Creacion(model));
                }
                else
                {
                    return BadRequest("El usuario logueado no tiene permisos para realizar la acción.");
                }
            }
            else 
            {
                return BadRequest("No se pudo verficar la autenticacion del usuario.");
            }
        }

        [HttpPut]
        [Route("ChEstate/{id}")]
        public IHttpActionResult CEst(int id)
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
                Rol R = DB.Rol.Where(x => x.Id == Id_Rol && x.Rol1.ToUpper().Equals("ADMINISTRADOR")).FirstOrDefault();
                if (R != null)
                {
                    return Ok(UT.ChangeEst(id));
                }
                else
                {
                    return BadRequest("El usuario logueado no tiene permisos para realizar la acción.");
                }
            }
            else 
            {
                return BadRequest("No se tiene constancia del logueo del usuario");
            }
        }

        [HttpPut]
        [Route("ChPass")]
        public IHttpActionResult CPass(UsuarioPass UP) 
        {
            int Id_Usuario;
            Response response = new Response();
            DB = new SlabEntities();
            if (!string.IsNullOrEmpty(UP.NombreUsuario) && !string.IsNullOrEmpty(UP.Correo) && !string.IsNullOrEmpty(UP.Password) && !string.IsNullOrEmpty(UP.NewPasswrd))
            {
                TokenGenerator TG = new TokenGenerator();
                string result = TG.DecodeTokenAwt(Request.Headers.Authorization.ToString().Replace("Bearer ", "")).Result;
                if (!string.IsNullOrEmpty(result))
                {
                    Id_Usuario = int.Parse(result.Split('|')[0]);
                    Usuario us = DB.Usuario.Where(x => x.Correo.ToUpper().Equals(UP.Correo) && x.Usuario1.ToUpper().Equals(UP.NombreUsuario) && x.Id == Id_Usuario).FirstOrDefault();
                    if (us != null)
                    {
                        if (response.Desencriptar(us.Contrasena) == UP.Password)
                        {
                            return Ok(UT.ChangePass(UP.NewPasswrd, us.Id));
                        }
                        else
                        {
                            return BadRequest("La contraseña actual no coincide.");
                        }
                    }
                    else
                    {
                        return BadRequest("El usuario logueado no corresponde al que se le cambiara la contraseña.");
                    }
                }
                else 
                {
                    return BadRequest("No se logro validar la autenticacion del usuario.");
                }
            }
            else 
            {
                return BadRequest("La informacion ingresada esta incorrecta.");
            }
        }
    }
}