using Encarta.Models;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace Encarta.ViewModels
{
    public class QrScannerViewModel : BaseViewModel
    {

        private readonly INavigation _navigation;
        public ICommand EscanearCommand { get; }
        public QrScannerViewModel(INavigation navigation)
        {
            _navigation = navigation;
            //ContinuarCommand = new Command(ContinuarAction);
            EscanearCommand = new Command(EscanearAction);
            OpenScanner();
        }

        private void EscanearAction(object obj)
        {
            OpenScanner();
        }

        private async void OpenScanner()
        {
            var scannerPage = new ZXingScannerPage();
            await _navigation.PushModalAsync(scannerPage);

            scannerPage.OnScanResult += (result) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (result.BarcodeFormat == BarcodeFormat.QR_CODE)
                    {
                        await _navigation.PopModalAsync();
                        CartaSingleton.Instance.NuevaCarta.UrlQR = result.Text;
                        await _navigation.PushAsync(new MapaPage());
                    }
                });
            };
            
        }

        //public async void OnScanResultAction(Result result)
        //{
        //    if (result.BarcodeFormat == BarcodeFormat.QR_CODE)
        //    {
        //        CartaSingleton.Instance.NuevaCarta.UrlQR = result.Text;
        //        Device.BeginInvokeOnMainThread(async () =>
        //        {
        //            await _navigation.PopAsync();
        //            await _navigation.PushAsync(new MapaPage());
        //        });
        //    }
        //}
    }
}
