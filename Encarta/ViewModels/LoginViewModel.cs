using Encarta.Models;
using Encarta.Services;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        public ICommand LoginCommand { get; }
        public ICommand CrearCuentaCommand { get; }

        private string _correo;
        public string Correo
        {
            get => _correo;
            set
            {
                SetProperty(ref _correo, value);
            }
        }

        private string _contrasenia;
        public string Contrasenia
        {
            get => _contrasenia;
            set
            {
                SetProperty(ref _contrasenia, value);
            }
        }

        private bool _contraseniaError;
        public bool ContraseniaError
        {
            get => _contraseniaError;
            set
            {
                SetProperty(ref _contraseniaError, value);
            }
        }

        private string _contraseniaErrorText;
        public string ContraseniaErrorText
        {
            get => _contraseniaErrorText;
            set
            {
                SetProperty(ref _contraseniaErrorText, value);
            }
        }

        private bool _correoError;
        public bool CorreoError
        {
            get => _correoError;
            set
            {
                SetProperty(ref _correoError, value);
            }
        }

        private bool _cuentaNoExiste = false;
        public bool CuentaNoExiste
        {
            get => _cuentaNoExiste;
            set
            {
                SetProperty(ref _cuentaNoExiste, value);
            }
        }

        public LoginViewModel(INavigation navigation)
        {
            _navigation = navigation;
            LoginCommand = new Command(async () => await LoginAction());
            CrearCuentaCommand = new Command(CrearCuentaAction);
        }

        internal async void PageAppearing()
        {
            if (Preferences.Get("id_usuario","none") != "none")
            {
                await _navigation.PopModalAsync();
            }
        }

        private void CrearCuentaAction(object obj)
        {
            _navigation.PushModalAsync(new RegisterPage());
        }

        private async Task LoginAction()
        {
            if (!ValidarDatos())
            {
                return;
            }

            IsBusy = true;
            if (await DataBaseControl.CheckUsuarioLogged(new Usuario { Correo = Correo }, Contrasenia))
            {
                Usuario usuarioLogeado = await DataBaseControl.GetUsuarioEmail(Correo);
                IsBusy = false;
                if (usuarioLogeado != null)
                {
                    if (usuarioLogeado.Bloqueado)
                    {
                        await Application.Current.MainPage.DisplayAlert("", "Tu cuenta está bloqueada por incumplir las normas de Encarta", "Vale");
                        LoginErrorVisual();
                        return;
                    }
                    UsuarioPreferencesControl.LogInPreferences(usuarioLogeado);
                    
                    await _navigation.PopModalAsync();
                    return;
                }
            }
            else
            {
                LoginErrorVisual();
            }
            IsBusy = false;
        }

        private void LoginErrorVisual()
        {
            Contrasenia = String.Empty;
            ContraseniaErrorText = "Introduce contraseña";
            ContraseniaError = true;
            CuentaNoExiste = true;
        }

        private bool ValidarDatos()
        {
            CuentaNoExiste = false;
            CorreoError = string.IsNullOrEmpty(Correo) || !Validador.IsValidEmail(Correo);
            ContraseniaError = string.IsNullOrEmpty(Contrasenia) || Contrasenia.Length < 6;


            if (CorreoError || ContraseniaError)
            {
                if (ContraseniaError)
                {
                    if (!string.IsNullOrEmpty(Contrasenia) && Contrasenia.Length < 6)
                    {
                        ContraseniaErrorText = "La contraseña debe tener 6 caracteres mínimo";
                    }
                    else
                    {
                        ContraseniaErrorText = "Introduce contraseña";
                    }
                }
                return false;
            }
            return true;
        }
    }
}
