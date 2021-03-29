using PruebaSlab.Models;
using PruebaSlab.Models.DB;
using PruebaSlab.Models.Proyecto;
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
    public class ProyectoTransaction
    {
        SlabEntities DB = null;
        Response response = null;
        public dynamic Index(int? id)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "En Proceso" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
            #endregion
            ProyectoIndex PI = new ProyectoIndex();
            UsuarioList UL = new UsuarioList();
            List<UsuarioList> LstUL = new List<UsuarioList>();
            try
            {
                DB = new SlabEntities();
                DB.Configuration.LazyLoadingEnabled = true;
                response = new Response();
                Rol R = DB.Rol.Where(y => y.Rol1.ToUpper().Equals("OPERADOR")).FirstOrDefault();
                List<Usuario> U = DB.Usuario.Where(x => x.Rol_Id == R.Id && x.Estado == true).ToList();
                Proyecto P = id != null ? DB.Proyecto.Where(x => x.Id == id).FirstOrDefault() : null;
                if (U != null)
                {
                    foreach (Usuario usr in U)
                    {
                        UL = new UsuarioList()
                        {
                            Id = usr.Id,
                            Nombre = usr.Nombre,
                            Apellido = usr.Apellido,
                            Correo = usr.Correo,
                            Usuario = usr.Usuario1,
                        };
                        LstUL.Add(UL);
                    }
                }
                PI = new ProyectoIndex()
                {
                    Id = P != null ? P.Id : 0,
                    Nombre = P != null ? P.Nombre : "",
                    Descripcion = P != null ? P.Descripcion : "",
                    FechaInicio = P != null ? P.Fecha_Inicio : DateTime.Now,
                    FechaFin = P != null ? P.Fecha_Fin : DateTime.Now,
                    Estado = P != null ? P.Estado == false ? 0 : 1 : 0,
                    LstEstado = LstEL
                };
                response.Successfully = true;
                response.Code = 200;
                response.Message = "Consulta realizada con éxito.";
                response.Result = PI;
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
        public dynamic List(int? id)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "En Proceso" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
            #endregion
            ProyectList PL = new ProyectList();
            List<ProyectList> LstPL = new List<ProyectList>();
            try
            {
                DB = new SlabEntities();
                DB.Configuration.LazyLoadingEnabled = true;
                response = new Response();
                List<Usuario> Usr = DB.Usuario.ToList();
                List<Proyecto> Pro = id == null ? DB.Proyecto.ToList() : DB.Proyecto.Where(x => x.Id == id).ToList();
                if (Pro.Count > 0)
                {
                    foreach (Proyecto P in Pro)
                    {
                        PL = new ProyectList()
                        {
                            Id = P.Id,
                            Nombre = P.Nombre,
                            Descripcion = P.Descripcion,
                            FechaInicio = P.Fecha_Inicio,
                            FechaFin = P.Fecha_Fin,
                            Id_Operario = P.Id_Operario,
                            Operario = Usr.Where(x => x.Id == P.Id_Operario).Select(x => x.Nombre + ' ' + x.Apellido).FirstOrDefault(),
                            Estado = P.Estado,
                            ValEstado = P.Estado == false ? "En Proceso" : "Finalizado"
                        };
                        LstPL.Add(PL);
                    }
                    response.Message = "";
                    response.Result = LstPL;
                }
                else
                {
                    response.Message = "No se encontro ningun Proyecto";
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
        public dynamic Creacion(ProyectoEdit model)
        {
            DB = new SlabEntities();
            DB.Configuration.LazyLoadingEnabled = true;
            response = new Response();
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "En Proceso" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
            #endregion
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    Proyecto Pro = DB.Proyecto.Where(x => x.Nombre.ToUpper().Equals(model.Nombre.ToUpper()) && x.Descripcion.ToUpper().Equals(model.Descripcion.ToUpper())).FirstOrDefault();
                    if (Pro == null)
                    {
                        Pro = new Proyecto()
                        {
                            Nombre = model.Nombre,
                            Descripcion = model.Descripcion,
                            Fecha_Inicio = model.FechaInicio,
                            Fecha_Fin = model.FechaFin,
                            Id_Operario = model.Id_Operario,
                            Estado = model.Estado == 0 ? false : true
                        };
                        DB.Proyecto.Add(Pro);
                        DB.SaveChanges();
                        response.Successfully = true;
                        response.Code = 201;
                        response.Message = "Proyecto Insertado con éxito.";
                        response.Result = "Successfully";
                        transaction.Commit();
                    }
                    else
                    {
                        response.Successfully = false;
                        response.Code = 200;
                        response.Message = string.Format("Ya existe un Proyecto con ese nombre {0} y con esa descripcion {1}", model.Nombre, model.Descripcion);
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
        public dynamic Actualizacion(ProyectoEdit model)
        {
            bool OkFechas = true;
            DB = new SlabEntities();
            DB.Configuration.LazyLoadingEnabled = true;
            response = new Response();
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    Proyecto PR = DB.Proyecto.Where(x => x.Id == model.Id).FirstOrDefault();
                    List<Tarea> T = DB.Tarea.Where(x => x.Id_Proyecto == model.Id).ToList();
                    if (PR != null)
                    {
                        foreach (Tarea tarea in T)
                        {
                            if (tarea.Fecha_Ejecucion > PR.Fecha_Fin)
                            {
                                OkFechas = false;
                            }
                        }
                        if (OkFechas)
                        {
                            PR.Nombre = model.Nombre;
                            PR.Descripcion = model.Descripcion;
                            PR.Fecha_Fin = model.FechaFin;
                            DB.Entry(PR).State = EntityState.Modified;
                            DB.SaveChanges();
                            response.Successfully = true;
                            response.Code = 200;
                            response.Message = "Actualización de datos realizada con éxito.";
                            response.Result = "Successfully";
                            transaction.Commit();
                        }
                        else
                        {
                            response.Successfully = false;
                            response.Code = 200;
                            response.Message = "La fecha final del proyecto no puede ser anterior a las tareas asignadas a este.";
                            response.Result = null;
                        }
                    }
                    else
                    {
                        response.Successfully = false;
                        response.Code = 200;
                        response.Message = "No se logró validar el Proyecto.";
                        response.Result = null;
                    }
                }
                catch (Exception Ex)
                {
                    response.Successfully = false;
                    response.Code = 500;
                    response.Message = Ex.Message.ToString();
                    response.Result = null;
                }
            }
            return response;
        }
        public dynamic Delete(int id)
        {
            DB = new SlabEntities();
            DB.Configuration.LazyLoadingEnabled = true;
            response = new Response();
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    List<Tarea> LT = DB.Tarea.Where(x => x.Id_Proyecto == id).ToList();
                    foreach (Tarea item in LT)
                    {
                        DB.Entry(item).State = EntityState.Deleted;
                        DB.SaveChanges();
                    }
                    Proyecto PR = DB.Proyecto.Where(x => x.Id == id).FirstOrDefault();
                    DB.Entry(PR).State = EntityState.Deleted;
                    DB.SaveChanges();
                    response.Successfully = true;
                    response.Code = 200;
                    response.Message = "Eliminacion del proyecto realizada con éxito.";
                    response.Result = "Successfully";
                    transaction.Commit();
                }
                catch (Exception Ex)
                {
                    response.Successfully = false;
                    response.Code = 500;
                    response.Message = Ex.Message.ToString();
                    response.Result = null;
                }
            }
            return response;
        }
        public dynamic ChangeEstado(int id)
        {
            DB = new SlabEntities();
            DB.Configuration.LazyLoadingEnabled = true;
            response = new Response();
            bool TComplete = true;
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {

                    Proyecto PR = DB.Proyecto.Where(x => x.Id == id).FirstOrDefault();
                    List<Tarea> T = DB.Tarea.Where(x => x.Id_Proyecto == id).ToList();
                    int R = DB.Rol.Where(x => x.Rol1.ToUpper().Equals("ADMINISTRADOR")).Select(x => x.Id).FirstOrDefault();
                    List<Usuario> LstU = DB.Usuario.Where(x => x.Rol_Id == R).ToList();
                    if (PR != null)
                    {
                        if (PR.Estado == false)
                        {
                            foreach (Tarea tarea in T)
                            {
                                if (tarea.Estado == false)
                                {
                                    TComplete = false;
                                }
                            }
                            if (TComplete)
                            {
                                PR.Estado = true;
                                DB.Entry(PR).State = EntityState.Modified;
                                DB.SaveChanges();
                                response.Successfully = true;
                                response.Code = 200;
                                response.Message = "Actualización del estad del proyecto realizado con éxito.";
                                response.Result = "Successfully";
                                #region Email
                                var msg = new MailMessage();
                                foreach (Usuario U in LstU)
                                {
                                    msg.To.Add(new MailAddress(U.Correo));
                                }
                                msg.From=new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                                string fromPassword = ConfigurationManager.AppSettings["MailPassword"];
                                msg.Subject = "Creacion de Usuario en SlabCode.";
                                msg.Body = "El proyecto " + PR.Nombre + " ha finalizado con exito.";

                                var smtp = new SmtpClient
                                {
                                    Host = "smtp.gmail.com",
                                    Port = 587,
                                    EnableSsl = true,
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = false,
                                    Credentials = new NetworkCredential(msg.From.Address, fromPassword)
                                };
                                smtp.Send(msg);
                                #endregion
                                transaction.Commit();
                            }
                            else
                            {
                                response.Successfully = false;
                                response.Code = 200;
                                response.Message = "Hay tareas por finalizar.";
                                response.Result = null;
                            }
                        }
                        else
                        {
                            response.Successfully = false;
                            response.Code = 200;
                            response.Message = "El proyecto ya esta finalizado.";
                            response.Result = null;
                        }
                    }
                    else
                    {
                        response.Successfully = false;
                        response.Code = 200;
                        response.Message = "No se logró validar el Proyecto.";
                        response.Result = null;
                    }
                }
                catch (Exception Ex)
                {
                    response.Successfully = false;
                    response.Code = 500;
                    response.Message = Ex.Message.ToString();
                    response.Result = null;
                }
            }
            return response;
        }
    }
}