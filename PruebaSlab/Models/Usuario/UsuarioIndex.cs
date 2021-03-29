using PruebaSlab.Models.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PruebaSlab.Models.Usuario
{
    public class UsuarioIndex
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public int Estado { get; set; }
        public List<EstadoList> ListaEstados { get; set; }
    }
}