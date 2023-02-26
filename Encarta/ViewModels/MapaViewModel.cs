using Encarta.Models;
using Encarta.Services;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Encarta.Views.Pages.PopUps;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Encarta.ViewModels
{
    public class MapaViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private Map map;

        public ICommand BackButtonCommand { get; }
        public ICommand ContinuarCommand { get; }

        public MapaViewModel(INavigation navigation, Map map)
        {
            _navigation = navigation;
            this.map = map;
            BackButtonCommand = new Command(BackButtonAction);
            ContinuarCommand = new Command(async () => await ContinuarAction());
        }

        private async Task ContinuarAction()
        {

            if (map.Pins.Count != 1)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Selecciona en el mapa la posición del restaurante", "Vale");
                return;
            }
            IsBusy = true;

            List<Restaurante> restCerca = await RestaurantesCerca(map.Pins.FirstOrDefault());

            IsBusy = false;
            if (restCerca.Count == 0)
            {
                await _navigation.PushAsync(new ResumenNuevaCartaPage(map.Pins.FirstOrDefault()));
                return;
            }

            await _navigation.PushPopupAsync(new RestaurantesCercaPopUp(_navigation, new ObservableCollection<Restaurante>(restCerca), map.Pins.FirstOrDefault()));
            return;
        }

        private async Task<List<Restaurante>> RestaurantesCerca(Pin pin)
        {
            Position pos = pin.Position;
            List<Restaurante> list = new List<Restaurante>();
            List<Restaurante> allRest = await DataBaseControl.GetAllRestaurantes();
            foreach (Restaurante r in allRest)
            {
                //var hola = DistanciaMetros(pos.Latitude, pos.Longitude, r.Latitude, r.Longitude); 
                if (DistanciaMetros(pos.Latitude, pos.Longitude, r.Latitude, r.Longitude) < 20)
                {
                    list.Add(r);
                }
            }
            return list;
        }

        private static double DistanciaMetros(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return (dist * 1.609344) * 1000; //sin *1000 para KM
        }

        public void MapClicked(MapClickedEventArgs obj)
        {
            Pin newPin = new Pin
            {
                Type = PinType.Place,
                Position = obj.Position,
                Label = "Nueva Carta"
            };
            map.Pins.Clear();
            map.Pins.Add(newPin);
        }

        public async void BackButtonAction(object obj)
        {
            bool cerrarPag = await Application.Current.MainPage.DisplayAlert("Atención", "Al salir se perderan los datos", "Salir", "Cancelar");
            if (!cerrarPag)
            {
                return;
            }

            var existingPages = _navigation.NavigationStack.ToList();
            foreach (var page in existingPages)
            {
                if (page != null)
                    _navigation.RemovePage(page);
            }
        }
    }
}
