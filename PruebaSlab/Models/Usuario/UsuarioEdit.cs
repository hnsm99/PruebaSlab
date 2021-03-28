using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PruebaSlab.Models.Usuario
{
    public class UsuarioEdit
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Rol_Id { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
    }
}