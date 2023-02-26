using Encarta.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Encarta.Utilidades
{
    public class UsuarioPreferencesControl
    {
        public static void LogOutPreferences()
        {
            Preferences.Set("id_usuario", "none");
            Preferences.Set("cuenta", "none");
            Preferences.Set("admin", false);
            Preferences.Set("nombre", "none");
        }

        public static void LogInPreferences(Usuario u)
        {
            Preferences.Set("id_usuario", u.Id);
            Preferences.Set("cuenta", u.Correo);
            Preferences.Set("admin", u.Admin);
            Preferences.Set("nombre", u.Nombre);
        }

        public static Usuario GetUsuarioInPreferences()
        {
            return new Usuario
            {
                Id = Preferences.Get("id_usuario", "none"),
                Nombre = Preferences.Get("nombre", "none"),
                Correo = Preferences.Get("cuenta", "none"),
                Admin = Preferences.Get("admin", false)
            };
        }

        public static bool IsUsuarioAdmin()
        {
            return Preferences.Get("admin", false);
        }
    }
}
