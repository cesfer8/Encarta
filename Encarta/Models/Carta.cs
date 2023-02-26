using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Encarta.Models
{
    public class Carta
    {
        public string Id { get; set; }
        public string Foto { get; set; }
        public string UrlQR { get; set; }
        public DateTime Fecha_creacion { get; set; }
        public string Id_usuario { get; set; }
        public string Id_restaurante { get; set; }
        public int Denuncias { get; set; }

        //prueba
        //public Stream FotoStream { get; set; }
        public FileResult FotoResult { get; set; }
    }
}