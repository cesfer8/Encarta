using Encarta.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Encarta.Utilidades
{
    public sealed class CartaSingleton
    {
        private static CartaSingleton instance = null;

        public Carta NuevaCarta;
        public Restaurante Restaurante;

        protected CartaSingleton()
        {
            //NuevaCarta = new Carta();
        }

        public static CartaSingleton Instance
        {
            get
            {
                if (instance == null)
                    instance = new CartaSingleton();

                return instance;
            }
        }

        public void Clear()
        {
            NuevaCarta = new Carta();
            Restaurante = null;
        }
    }
}
