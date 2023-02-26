using Encarta.Models;
using Encarta.Services;
using Encarta.Utilidades;
using Encarta.ViewModels;
using Encarta.Views.Pages;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace Encarta
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            CheckUserRegister();
        }

        private async void CheckUserRegister()
        {
            string correo = Preferences.Get("cuenta", "none");
            if (correo == "none")
            {
                await Navigation.PushModalAsync(new LoginPage());
            }
            else
            {
                Usuario u = await DataBaseControl.GetUsuarioEmail(correo);
                if (u.Bloqueado)
                {
                    UsuarioPreferencesControl.LogOutPreferences();
                    await Navigation.PushModalAsync(new LoginPage());
                }
            }
        }
    }
}
