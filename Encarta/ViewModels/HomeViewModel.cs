using Encarta.Models;
using Encarta.Services;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Encarta.Views.Pages.PopUps;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        public ICommand BuscarPulsadoCommand { get; }
        public ICommand SubirQrCommand { get; }
        public ICommand SubirFotoCommand { get; }
        public ICommand AdministrarCartasCommand { get; }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                SetProperty(ref _isAdmin, value);
            }
        }

        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
            BuscarPulsadoCommand = new Command(BuscarPulsadoAction);
            SubirQrCommand = new Command(SubirQrAction);
            SubirFotoCommand = new Command(SubirFotoAction);
            AdministrarCartasCommand = new Command(AdministrarCartasAction);
        }

        internal void OnAppearing()
        {
            IsAdmin = UsuarioPreferencesControl.IsUsuarioAdmin();
        }

        private async void AdministrarCartasAction(object obj)
        {
            await _navigation.PushAsync(new VerCartasReportadasPage());
        }

        private async void SubirFotoAction(object obj)
        {
            CartaSingleton.Instance.Clear();
            await _navigation.PushAsync(new FotoCartaPage());
        }

        private async void BuscarPulsadoAction(object obj)
        {
            await _navigation.PushAsync(new BuscarRestaurantePage());
        }

        private async void SubirQrAction(object obj)
        {
            CartaSingleton.Instance.Clear();
            await _navigation.PushAsync(new QrScannerPage());
        }

        
    }
}