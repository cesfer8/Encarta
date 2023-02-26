using Encarta.Models;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class RestaurantesCercaPopUpViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private Xamarin.Forms.Maps.Pin _pin;
        public ICommand FramePulsadoCommand { get; }
        public ICommand OtroRestauranteCommand { get; }

        private ObservableCollection<Restaurante> _listadoRestaurantes;
        public ObservableCollection<Restaurante> ListadoRestaurantes
        {
            get => _listadoRestaurantes;
            set
            {
                SetProperty(ref _listadoRestaurantes, value);
            }
        }

        public RestaurantesCercaPopUpViewModel(INavigation navigation, ObservableCollection<Restaurante> listaRestaurantes, Xamarin.Forms.Maps.Pin pin)
        {
            _navigation = navigation;
            _pin = pin;
            ListadoRestaurantes = listaRestaurantes;
            FramePulsadoCommand = new Command<Restaurante>(FramePulsadoAction);
            OtroRestauranteCommand = new Command(OtroRestauranteAction);
            //ContinuarCommand = new Command(async () => await ContinuarAction());
        }

        private async void OtroRestauranteAction(object obj)
        {
            CartaSingleton.Instance.Restaurante = null;
            _navigation.PushAsync(new ResumenNuevaCartaPage(_pin));
            await _navigation.PopPopupAsync();
        }

        private async void FramePulsadoAction(Restaurante restaurantePulsado)
        {
            CartaSingleton.Instance.Restaurante = restaurantePulsado;
            _navigation.PushAsync(new ResumenNuevaCartaPage());
            await _navigation.PopPopupAsync();
        }
    }
}
