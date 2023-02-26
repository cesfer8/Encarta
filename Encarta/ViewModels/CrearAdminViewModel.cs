using Encarta.Services;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class CrearAdminViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public ICommand AceptarCommand { get; }

        private string _correoIntroducido;
        public string CorreoIntroducido
        {
            get => _correoIntroducido;
            set
            {
                SetProperty(ref _correoIntroducido, value);
            }
        }

        public CrearAdminViewModel(INavigation navigation)
        {
            _navigation = navigation;
            AceptarCommand = new Command(AceptarAction);
        }

        private async void AceptarAction()
        {
            bool crearAdmin = await Application.Current.MainPage.DisplayAlert("Atención", "El usuario será administrador", "Continuar", "Cancelar");
            if (!crearAdmin)
            {
                return;
            }
            IsBusy = true;
            bool creado = await DataBaseControl.AddAdminToUser(CorreoIntroducido);
            IsBusy = false;
            if (creado)
            {
                await Application.Current.MainPage.DisplayAlert("", CorreoIntroducido+" es Administrador", "Vale");
                await _navigation.PopPopupAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("", "El correo no corresponde a un usuario", "Vale");
                CorreoIntroducido = String.Empty;
            }
        }
    }
}
