using Encarta.Utilidades;
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
    public partial class ResumenNuevaCartaPage : ContentPage
    {
        private ResumenNuevaCartaViewModel vm;
        private Pin _pin;
        public ResumenNuevaCartaPage(Pin pin = null)
        {
            InitializeComponent();
            vm = new ResumenNuevaCartaViewModel(Navigation, pin);
            BindingContext = vm;
            _pin = pin;
            ColocarMapa();
        }

        private void ColocarMapa()
        {
            map.Pins.Clear();
            Pin newPin = new Pin();
            newPin.Label = "Restaurante";
            Position pos = new Position();
            if (CartaSingleton.Instance.Restaurante == null)
            {
                pos = _pin.Position;
            }
            else
            {
                pos = new Position(CartaSingleton.Instance.Restaurante.Latitude, CartaSingleton.Instance.Restaurante.Longitude);
            }
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