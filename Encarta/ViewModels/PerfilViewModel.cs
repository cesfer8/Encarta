using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Xamarin.Essentials;
using Encarta.Models;
using Encarta.Services;
using Encarta.Views.Pages.PopUps;
using Rg.Plugins.Popup.Extensions;

namespace Encarta.ViewModels
{
    public class PerfilViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public ICommand CerrarSesionCommand { get; }
        public ICommand SobreNosotrosCommand { get; }
        public ICommand CrearAdminCommand { get; }

        private Usuario _usuario;
        public Usuario Usuario
        {
            get => _usuario;
            set
            {
                SetProperty(ref _usuario, value);
            }
        }

        private int _numCartas;
        public int NumCartas
        {
            get => _numCartas;
            set
            {
                SetProperty(ref _numCartas, value);
            }
        }

        public PerfilViewModel(INavigation navigation)
        {
            _navigation = navigation;
            CerrarSesionCommand = new Command(CerrarSesionAction);
            SobreNosotrosCommand = new Command(SobreNosotrosAction);
            CrearAdminCommand = new Command(CrearAdminAction);
        }

        private async void CrearAdminAction(object obj)
        {
            await _navigation.PushPopupAsync(new CrearAdminPopUp(_navigation));
        }

        private async void SobreNosotrosAction(object obj)
        {
            await _navigation.PushPopupAsync(new SobreNosotrosPopUp());
        }

        internal async void PageAppearing()
        {
            Usuario = UsuarioPreferencesControl.GetUsuarioInPreferences();
            NumCartas = (await DataBaseControl.GetCartasFromUsuario(Usuario.Id)).Count;
        }

        private async void CerrarSesionAction(object obj)
        {
            bool cerrarSesion = await Application.Current.MainPage.DisplayAlert("Atención", "Se cerrara tu sesión. ¿Estas seguro?", "Si", "No");
            if (cerrarSesion)
            {
                UsuarioPreferencesControl.LogOutPreferences();
                await _navigation.PushModalAsync(new LoginPage());
            }

        }
    }
}
