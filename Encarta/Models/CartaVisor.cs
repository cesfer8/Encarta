using System;
using System.Collections.Generic;
using System.Text;

namespace Encarta.Models
{
    public class CartaVisor
    {
        public string Id { get; set; }
        public bool IsQR { get; set; }
        public string Foto { get; set; }
        public string UrlQR { get; set; }
        public string id_usuario { get; set; }
        public DateTime Fecha_creacion { get; set; }
        public bool IsFav { get; set; }
        public int Denuncias { get; set; }
        public Restaurante Restaurante { get; set; }
        //public string Icono
        //{
        //    get
        //    {
        //        if (this.IsQR)
        //        {
        //            return "&#xE806;";
        //        }
        //        return "&#xE807;";
        //    }
        //}

        public string CuantosDiasHace
        {
            get
            {
                int dias = (int)(DateTime.Today - Fecha_creacion).TotalDays;
                return "Hace " + dias + " dias";
            }
        }

        public string CuantasDenuncias
        {
            get
            {
                return Denuncias+" Denuncias";
            }
        }
    }
}
