using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PruebaSlab.Utilities
{
    public class Response
    {
        public bool Successfully { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        [NotMapped]
        public dynamic Result { get; set; }

        public string Encriptar(string Cadena)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(Cadena);
            result = Convert.ToBase64String(encryted);
            return result;
        }
    }
}