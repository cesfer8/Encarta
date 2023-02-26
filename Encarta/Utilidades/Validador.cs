using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Encarta.Utilidades
{
    public static class Validador
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidName(string name)
        {
            Regex nm = new Regex(@"^[a-z A-Z]{2,120}$");
            if (!nm.IsMatch(name))
            {
                return false;
            }
            return true;
        }
    }
    
}
