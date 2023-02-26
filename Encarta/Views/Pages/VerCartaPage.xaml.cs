using Encarta.Models;
using Encarta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Encarta.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerCartaPage : ContentPage
    {
        VerCartaViewModel vm;
        private CartaVisor _carta;
        public VerCartaPage(CartaVisor carta, bool IsAdmin = false)
        {
            InitializeComponent();
            vm = new VerCartaViewModel(Navigation, carta, IsAdmin);
            BindingContext = vm;
            _carta = carta;
            ColocarMapa();
        }
        private void ColocarMapa()
        {
            map.Pins.Clear();
            Pin newPin = new Pin();
            newPin.Label = _carta.Restaurante.Nombre;
            Position pos = new Position();
            pos = new Position(_carta.Restaurante.Latitude, _carta.Restaurante.Longitude);
            
            newPin.Position = pos;
            map.Pins.Add(newPin);
            MapSpan mapSpan = new MapSpan(pos, 1, 1);

            map.MoveToRegion(mapSpan.WithZoom(800));
        }


        private async void qrFrame_Tapped(object sender, EventArgs e)
        {
            var scaleUp = qrFrame.ScaleTo(0.95, 50);
            var fadeOut = qrFrame.FadeTo(0.8, 70);
            await Task.WhenAll(fadeOut, scaleUp);
            qrFrame.ScaleTo(1, 70);
            qrFrame.FadeTo(1, 70);
        }
    }
}