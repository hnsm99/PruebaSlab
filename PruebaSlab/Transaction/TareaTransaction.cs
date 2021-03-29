using PruebaSlab.Models;
using PruebaSlab.Models.DB;
using PruebaSlab.Models.Proyecto;
using PruebaSlab.Models.Tarea;
using PruebaSlab.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PruebaSlab.Transaction
{
    public class TareaTransaction
    {
        SlabEntities DB = null;
        Response response = null;
        public dynamic Index(int? id)
        {
            EstadoList EL = new EstadoList();
            List<EstadoList> LstEL = new List<EstadoList>();
            TareaIndex TI = new TareaIndex();
            ProyectList PL = new ProyectList();
            List<ProyectList> LstPL = new List<ProyectList>();
            try
            {
                DB = new SlabEntities();
                DB.Configuration.LazyLoadingEnabled = true;
                response = new Response();
                LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Pendiente" });
                LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Realizada" });
                List<Proyecto> P = DB.Proyecto.ToList();
                List<Usuario> U = DB.Usuario.ToList();
                Tarea T = id != null ? DB.Tarea.Where(x => x.Id == id).FirstOrDefault() : null;
                if (P != null)
                {
                    foreach (Proyecto p in P)
                    {
                        PL = new ProyectList()
                        {
                            Id = p.Id,
                            Nombre = p.Nombre,
                            Descripcion = p.Descripcion,
                            FechaInicio = p.Fecha_Inicio,
                            FechaFin = p.Fecha_Fin,
                            Id_Operario = p.Id_Operario,
                            Operario = U.Where(x => x.Id == p.Id_Operario).Select(x => x.Nombre + ' ' + x.Apellido).FirstOrDefault(),
                            Estado = p.Estado,
                            ValEstado = p.Estado == false ? "En Proceso" : "Finalizado"
                        };
                        LstPL.Add(PL);
                    }
                }
                TI = new TareaIndex()
                {
                    Id = T != null ? T.Id : 0,
                    Nombre = T != null ? T.Nombre : "",
                    Descripcion = T != null ? T.Descripcion : "",
                    Fecha_Ejecucion = T != null ? T.Fecha_Ejecucion : DateTime.Now,
                    Id_Proyecto = T != null ? T.Id_Proyecto : 0,
                    LstProyecto = LstPL,
                    Estado = T != null ? T.Estado == false ? 0 : 1 : 0,
                    ListaEstado = LstEL
                };
                response.Successfully = true;
                response.Code = 200;
                response.Message = "Consulta realizada con éxito.";
                response.Result = TI;
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
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Pendiente" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Realizada" });
            #endregion
            TareaList TL = new TareaList();
            List<TareaList> LstTL = new List<TareaList>();
            try
            {
                DB = new SlabEntities();
                DB.Configuration.LazyLoadingEnabled = true;
                response = new Response();
                List<Proyecto> Pro = DB.Proyecto.ToList();
                List<Tarea> Tar = id == null ? DB.Tarea.ToList() : DB.Tarea.Where(x => x.Id == id).ToList();
                if (Tar.Count > 0)
                {
                    foreach (Tarea T in Tar)
                    {
                        TL = new TareaList()
                        {
                            Id = T.Id,
                            Nombre = T.Nombre,
                            Descripcion = T.Descripcion,
                            Fecha_Ejecucion = T.Fecha_Ejecucion,
                            Id_Proyecto = T.Id_Proyecto,
                            Proyecto = Pro.Where(x => x.Id == T.Id_Proyecto).Select(x => x.Nombre).FirstOrDefault(),
                            Estado = T.Estado == false ? 0 : 1,
                            ValEstado = T.Estado == false ? "Pendiente" : "Realizada"

                        };
                        LstTL.Add(TL);
                    }
                    response.Message = "";
                    response.Result = LstTL;
                }
                else
                {
                    response.Message = "No se encontro ninguna tarea";
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
        public dynamic Creacion(TareaEdit model)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "En Proceso" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
            #endregion
            DB = new SlabEntities();
            DB.Configuration.LazyLoadingEnabled = true;
            response = new Response();
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    Tarea T = DB.Tarea.Where(x => x.Nombre.ToUpper().Equals(model.Nombre.ToUpper()) && x.Descripcion.ToUpper().Equals(model.Descripcion.ToUpper()) && x.Id_Proyecto == model.Id_Proyecto).FirstOrDefault();
                    Proyecto Pro = DB.Proyecto.Where(x => x.Id == model.Id_Proyecto).FirstOrDefault();
                    if (T == null)
                    {
                        if (Pro.Fecha_Fin >= model.Fecha_Ejecucion && Pro.Fecha_Inicio <= model.Fecha_Ejecucion)
                        {
                            var ValEstado = LstEL.Where(x => x.Id == model.Estado).FirstOrDefault();
                            if (ValEstado != null)
                            {
                                T = new Tarea()
                                {
                                    Nombre = model.Nombre,
                                    Descripcion = model.Descripcion,
                                    Fecha_Ejecucion = model.Fecha_Ejecucion,
                                    Id_Proyecto = model.Id_Proyecto,
                                    Estado = model.Estado == 1 ? true : false
                                };
                                DB.Tarea.Add(T);
                                DB.SaveChanges();
                                response.Successfully = true;
                                response.Code = 201;
                                response.Message = "Tarea Insertada con éxito.";
                                response.Result = "Successfully";
                                transaction.Commit();
                            }
                            else
                            {
                                response.Successfully = false;
                                response.Code = 403;
                                response.Message = "El estado enviado no existe.";
                                response.Result = "";
                            }
                        }
                        else
                        {
                            response.Successfully = false;
                            response.Code = 403;
                            response.Message = "La fecha de ejecucion de la tarea excede la fecha de inicio/finalizacion del proyecto o es menor a la fecha actual.";
                            response.Result = "";
                        }
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
        public dynamic Actualizacion(TareaEdit model)
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
                    Proyecto PR = DB.Proyecto.Where(x => x.Id == model.Id_Proyecto).FirstOrDefault();
                    Tarea T = DB.Tarea.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (T != null)
                    {
                        if (PR.Fecha_Fin >= model.Fecha_Ejecucion)
                        {
                            T.Nombre = model.Nombre;
                            T.Descripcion = model.Descripcion;
                            T.Fecha_Ejecucion = model.Fecha_Ejecucion;
                        }
                        DB.Entry(T).State = EntityState.Modified;
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
                        response.Message = "No se logró validar la Tarea.";
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
        public dynamic Delete(int? id)
        {
            DB = new SlabEntities();
            DB.Configuration.LazyLoadingEnabled = true;
            response = new Response();
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    Tarea T = DB.Tarea.Where(x => x.Id == id).FirstOrDefault();
                    if (T != null)
                    {
                        DB.Entry(T).State = EntityState.Deleted;
                        DB.SaveChanges();
                        response.Successfully = true;
                        response.Code = 200;
                        response.Message = "Eliminacion del Tarea realizada con éxito.";
                        response.Result = "Successfully";
                        transaction.Commit();
                    }
                    else
                    {
                        response.Successfully = true;
                        response.Code = 400;
                        response.Message = "No se pudo validar la tarea";
                        response.Result = "";
                        transaction.Commit();
                    }
                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
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
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                   
                    Tarea T = DB.Tarea.Where(x => x.Id == id).FirstOrDefault();
                    if (T != null)
                    {
                        if (T.Estado == false)
                        {
                            T.Estado = true;
                            DB.Entry(T).State = EntityState.Modified;
                            DB.SaveChanges();
                            response.Successfully = true;
                            response.Code = 200;
                            response.Message = "Actualización de Estado de la tarea realizada con éxito.";
                            response.Result = "Successfully";
                            transaction.Commit();
                        }
                        else
                        {
                            response.Successfully = false;
                            response.Code = 200;
                            response.Message = "La tarea ya se encuentra finalizada";
                            response.Result = null;
                        }
                    }
                    else
                    {
                        response.Successfully = false;
                        response.Code = 200;
                        response.Message = "No se logró validar la Tarea.";
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