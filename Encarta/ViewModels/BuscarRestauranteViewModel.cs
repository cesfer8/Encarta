using Encarta.Models;
using Encarta.Services;
using Encarta.Views.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Encarta.ViewModels
{
    public class BuscarRestauranteViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private Map _map;
        private List<Restaurante> _listadoRestaurantes;
        private Restaurante _restauranteSeleccionado = null;
        public ICommand VerCartasCommand { get; }

        private IList<Pin> _listadoPins;
        public IList<Pin> ListadoPins
        {
            get => _listadoPins;
            set
            {
                SetProperty(ref _listadoPins, value);
            }
        }

        private string _textoBoton = "Selecciona restaurante";
        public string TextoBoton
        {
            get => _textoBoton;
            set
            {
                SetProperty(ref _textoBoton, value);
            }
        }



        public BuscarRestauranteViewModel(INavigation navigation, Map map)
        {
            _navigation = navigation;
            _map = map;
            VerCartasCommand = new Command(async() => await VerCartasAction());
            //BuscarPulsadoAction(new Object());
            CargarRestaurantes();
        }

        private async Task VerCartasAction()
        {
            if(_restauranteSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Selecciona restaurante para ver las cartas", "Vale");
                return;
            }

            await _navigation.PushAsync(new VerCartasPage(_restauranteSeleccionado));
        }

        private async void CargarRestaurantes()
        {
            IsBusy = true;
            _listadoRestaurantes = await DataBaseControl.GetAllRestaurantes();
            foreach(Restaurante r in _listadoRestaurantes)
            {
                Position pos = new Position(r.Latitude, r.Longitude);
                Pin pin = new Pin
                {
                    Position = pos,
                    Label = r.Nombre,
                    Type = PinType.Place,
                    
                };
                pin.MarkerClicked += PinClicked;
                _map.Pins.Add(pin);
            }
            IsBusy = false;
        }

        private void PinClicked(object sender, PinClickedEventArgs e)
        {
            int index = _map.Pins.IndexOf(sender as Pin);
            _restauranteSeleccionado = _listadoRestaurantes[index];
            TextoBoton = "Ver " + _restauranteSeleccionado.Nombre;
        }
    }
}
