using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PruebaSlab.Models.Proyecto
{
    public class ProyectList
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Id_Operario { get; set; }
        public string Operario { get; set; }
        public bool Estado { get; set; }
        public string ValEstado { get; set; }
    }
}