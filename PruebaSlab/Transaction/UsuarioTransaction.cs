using PruebaSlab.Models;
using PruebaSlab.Models.DB;
using PruebaSlab.Models.Rol;
using PruebaSlab.Models.Usuario;
using PruebaSlab.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace PruebaSlab.Transaction
{

    public class UsuarioTransaction
    {
        SlabEntities DB = null;
        Response response = null;

        public dynamic Index(int? id)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Inactivo" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Activo" });
            #endregion
            UsuarioIndex UI = new UsuarioIndex();
            List<UsuarioIndex> LstI = new List<UsuarioIndex>();
            try
            {
                DB = new SlabEntities();
                DB.Configuration.LazyLoadingEnabled = true;
                response = new Response();
                Usuario U = id != null ? DB.Usuario.Where(x => x.Id == id).FirstOrDefault() : null;
                UI = new UsuarioIndex()
                {
                    Id = U != null ? U.Id : 0,
                    Nombre = U != null ? U.Nombre : "",
                    Apellido = U != null ? U.Apellido : "",
                    Usuario = U != null ? U.Usuario1 : "",
                    Correo = U != null ? U.Correo : "",
                    Estado = 0,
                    ListaEstados = LstEL
                };
                response.Successfully = true;
                response.Code = 200;
                response.Message = "Consulta realizada con éxito.";
                response.Result = UI;
            }
            catch (Exception Exc)
            {
                response.Successfully = false;
                response.Code = 500;
                response.Message = Exc.Message.ToString();
                response.Result = null;
            }
            return response;
        }
        public dynamic IndexPass()
        {
            UsuarioPass UP = new UsuarioPass();
            Response response = new Response();
            response.Code = 200;
            response.Successfully = true;
            response.Message = "";
            response.Result = UP;
            return UP;
        }
        public dynamic List(int? id)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Inactivo" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Activo" });
            #endregion
            UsuarioList UL = new UsuarioList();
            List<UsuarioList> LstUL = new List<UsuarioList>();
            try
            {
                DB = new SlabEntities();
                DB.Configuration.LazyLoadingEnabled = true;
                response = new Response();
                List<Rol> R = DB.Rol.ToList();
                List<Usuario> Usr = id == null ? DB.Usuario.ToList() : DB.Usuario.Where(x => x.Id == id).ToList();
                if (Usr.Count > 0)
                {
                    foreach (Usuario U in Usr)
                    {
                        UL = new UsuarioList()
                        {
                            Id = U.Id,
                            Nombre = U.Nombre,
                            Apellido = U.Apellido,
                            Correo = U.Correo,
                            Rol_Id = U.Rol_Id,
                            Rol = R?.Where(x => x.Id == U.Rol_Id).Select(x => x.Rol1).FirstOrDefault(),
                            Usuario = U.Usuario1,
                            Estado = U.Estado == false ? "Inactivo" : "Activo"
                        };
                        LstUL.Add(UL);
                    }
                    response.Message = "";
                    response.Result = LstUL;
                }
                else
                {
                    response.Message = "No se encontro ningun Usuario";
                    response.Result = null;
                }
                response.Successfully = true;
                response.Code = 200;
            }
            catch (Exception Exc)
            {
                response.Successfully = false;
                response.Code = 500;
                response.Message = Exc.Message.ToString();
                response.Result = null;
            }
            return response;
        }
        public dynamic Creacion(UsuarioEdit model)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Inactivo" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Activo" });
            #endregion
            DB = new SlabEntities();
            response = new Response();
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    Usuario User = DB.Usuario.Where(x => x.Usuario1.ToUpper().Equals(model.Usuario.ToUpper()) && x.Correo.ToUpper().Equals(model.Correo.ToUpper()) && x.Rol_Id == model.Rol_Id).FirstOrDefault();
                    if (User == null&&LstEL.Where(x=>x.Id==model.Estado).FirstOrDefault()!=null)
                    {
                        User = new Usuario()
                        {
                            Nombre = model.Nombre,
                            Apellido = model.Apellido,
                            Rol_Id = DB.Rol.Where(x=>x.Rol1.ToUpper().Equals("OPERARIO")).Select(x=>x.Id).FirstOrDefault(),
                            Usuario1 = model.Usuario,
                            Correo = model.Correo,
                            Contrasena = response.Encriptar("Slab"+model.Usuario+"Code"),
                            Estado = model.Estado == 0 ? false : true
                        };
                        DB.Usuario.Add(User);
                        DB.SaveChanges();
                        #region Email
                        var fromAddress = new MailAddress(ConfigurationManager.AppSettings["MailFrom"], "Slab Code");
                        var toAddress = new MailAddress(model.Correo, model.Nombre + ' ' + model.Apellido);
                        string fromPassword = ConfigurationManager.AppSettings["MailPassword"];
                        const string subject = "Creacion de Usuario en SlabCode.";
                        string body = "Su contraseña es: " + "Slab" + model.Usuario + "Code";

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                        };
                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = subject,
                            Body = body
                        })
                        {
                            smtp.Send(message);
                        }
                        #endregion
                        response.Successfully = true;
                        response.Code = 201;
                        response.Message = "Usuario Insertado con éxito.";
                        response.Result = "Successfully";
                        transaction.Commit();
                    }
                    else
                    {
                        response.Successfully = false;
                        response.Code = 200;
                        response.Message = string.Format("Ya existe un Usuario con ese usuario {0}, con ese correo {1} o la informacion enviada es incorrecta.", model.Usuario, model.Correo);
                        response.Result = null;
                    }
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    response.Successfully = false;
                    response.Code = 400;
                    response.Message = "Error In the Method, Error: " + Exc.Message.ToString();
                    response.Result = "Error in the Method";
                }
            }
            return response;
        }
        public dynamic ChangePass(string newpass, int id)
        {
            DB = new SlabEntities();
            response = new Response();
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    Usuario Usr = DB.Usuario.Where(x => x.Id == id).FirstOrDefault();
                    Usr.Contrasena = response.Encriptar(newpass);
                    DB.Entry(Usr).State = EntityState.Modified;
                    DB.SaveChanges();
                    response.Successfully = true;
                    response.Code = 200;
                    response.Message = "Actualización de datos realizada con éxito.";
                    response.Result = "Successfully";
                    transaction.Commit();
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    response.Successfully = false;
                    response.Code = 400;
                    response.Message = "Error In the Method, Error: " + Exc.Message.ToString();
                    response.Result = "Error in the Method";
                }
            }
            return response;
        }
        public dynamic ChangeEst(int id)
        {
            DB = new SlabEntities();
            response = new Response();
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    Usuario Usr = DB.Usuario.Where(x => x.Id == id).FirstOrDefault();
                    if (Usr != null)
                    {
                        Usr.Estado = Usr.Estado == false ? true : false;
                        DB.Entry(Usr).State = EntityState.Modified;
                        DB.SaveChanges();
                        response.Successfully = true;
                        response.Code = 200;
                        response.Message = "Actualización de datos realizada con éxito.";
                        response.Result = "Successfully";
                        transaction.Commit();
                    }
                    else 
                    {
                        response.Successfully = true;
                        response.Code = 404;
                        response.Message = "Usuario no encontrado.";
                        response.Result = "";
                    }
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    response.Successfully = false;
                    response.Code = 400;
                    response.Message = "Error In the Method, Error: " + Exc.Message.ToString();
                    response.Result = "Error in the Method";
                }
            }
            return response;
        }
    }
}