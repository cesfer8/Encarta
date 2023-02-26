using Encarta.Models;
using Encarta.Utilidades;
using Encarta.ViewModels;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace Encarta.Views.Pages.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormatoCartaPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public FormatoCartaPopUp()
        {
            InitializeComponent();
        }

        private async void QR_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new QrScannerPage());
            await Navigation.PopPopupAsync();
        }

        private async void Foto_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FotoCartaPage());
            await Navigation.PopPopupAsync();
        }
    }
}