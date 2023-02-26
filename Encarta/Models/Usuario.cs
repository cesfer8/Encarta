using System;
using System.Collections.Generic;
using System.Text;

namespace Encarta.Models
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public List<string> CartasFavoritas { get; set; }
        public bool Admin { get; set; }
        public bool Bloqueado { get; set; }
    }
}
 