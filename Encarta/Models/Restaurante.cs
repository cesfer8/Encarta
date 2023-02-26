using System;
using System.Collections.Generic;
using System.Text;

namespace Encarta.Models
{
    public class Restaurante
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Id_usuario { get; set; }
    }
}
