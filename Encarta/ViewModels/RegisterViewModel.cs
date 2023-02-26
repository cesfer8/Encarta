using Encarta.Models;
using Encarta.Services;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Firebase.Auth;
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
    public class RegisterViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        public ICommand RegisterCommand { get; }
        public ICommand OpenLoginCommand { get; }


        private Usuario _usuario = new Usuario();
        public Usuario Usuario
        {
            get => _usuario;
            set
            {
                SetProperty(ref _usuario, value);
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

        private bool _nombreError;
        public bool NombreError
        {
            get => _nombreError;
            set
            {
                SetProperty(ref _nombreError, value);
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

        private bool _cuentaYaExiste = false;
        public bool CuentaYaExiste
        {
            get => _cuentaYaExiste;
            set
            {
                SetProperty(ref _cuentaYaExiste, value);
            }
        }

        public RegisterViewModel(INavigation navigation)
        {
            _navigation = navigation;
            RegisterCommand = new Command(async () => await RegisterAction());
            OpenLoginCommand = new Command(OpenLoginAction);
        }

        private void OpenLoginAction(object obj)
        {
            _navigation.PopModalAsync();
        }

        private async Task RegisterAction()
        {
            if (!ValidarDatos())
            {
                return;
            }

            IsBusy = true;
            if (await DataBaseControl.RegisterUsuario(Usuario, Contrasenia))
            {

                Usuario usuarioLogeado = await DataBaseControl.GetUsuarioEmail(Usuario.Correo);
                if (usuarioLogeado != null)
                {
                    UsuarioPreferencesControl.LogInPreferences(usuarioLogeado);
                    IsBusy = false;
                    await _navigation.PopModalAsync();
                }

            }
            else
            {
                Contrasenia = string.Empty;
                Usuario.Correo = string.Empty;
                ContraseniaErrorText = "Introduce contraseña";
                ContraseniaError = true;
                CuentaYaExiste = true;
            }
            IsBusy = false;
        }

        private bool ValidarDatos()
        {
            CuentaYaExiste = false;

            CorreoError = string.IsNullOrEmpty(Usuario.Correo) || !Validador.IsValidEmail(Usuario.Correo);
            NombreError = string.IsNullOrEmpty(Usuario.Nombre) || !Validador.IsValidName(Usuario.Nombre);
            ContraseniaError = string.IsNullOrEmpty(Contrasenia) || Contrasenia.Length < 6;


            if (CorreoError || ContraseniaError || NombreError)
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
