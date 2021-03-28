using PruebaSlab.Models;
using PruebaSlab.Models.DB;
using PruebaSlab.Models.Proyecto;
using PruebaSlab.Models.Usuario;
using PruebaSlab.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PruebaSlab.Transaction
{
    public class ProyectoTransaction
    {
        SlabEntities DB = null;
        Response response = null;
        public dynamic Index(int? id) {
            EstadoList EL = new EstadoList();
            List<EstadoList> LstEL = new List<EstadoList>();
            ProyectoIndex PI = new ProyectoIndex();
            UsuarioList UL = new UsuarioList();
            List<UsuarioList> LstUL = new List<UsuarioList>();
            try
            {
                DB = new SlabEntities();
                DB.Configuration.LazyLoadingEnabled = true;
                response = new Response();
                LstEL.Add(EL=new EstadoList() { Id = 0, Estado = "En Proceso" });
                LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
                List<Usuario> U = null;//DB.Usuario.Where(x => x.Rol_Id == DB.Rol.Where(y=>y.Rol1.ToUpper().Equals("OPERADOR")).Select(y=>y.Id).FirstOrDefault()&&x.Estado.Equals(1)).ToList();
                Proyecto P = id != null ? DB.Proyecto.Where(x => x.Id == id).FirstOrDefault() : null;
                if (U!=null)
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
                    FechaInicio=P!=null?P.Fecha_Inicio:DateTime.Now,
                    FechaFin=P!=null?P.Fecha_Fin:DateTime.Now,
                    Id_Operario=P!=null?P.Id_Operario:0,
                    Estado=P!=null?P.Estado:false,
                    LstEstado= LstEL,
                    LstOperadores=LstUL
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
    }
}