using Encarta.Services;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class FotoCartaViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public ICommand ContinuarCommand { get; }
        public ICommand SacarFotoCommand { get; }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        public FotoCartaViewModel(INavigation navigation)
        {
            _navigation = navigation;
            ContinuarCommand = new Command(ContinuarAction);
            SacarFotoCommand = new Command(SacarFotoAction);

            SacarFotoAction();
        }

        private async void SacarFotoAction()
        {
            try
            {
                var file = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();
                if (file == null)
                {
                    if(this.ImageSource == null)
                    {
                        await _navigation.PopAsync();
                    }
                    return;
                }
                    
                
                
                NuevaFoto(file);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void NuevaFoto(FileResult file)
        {
            Stream fileStream = await file.OpenReadAsync();
            this.ImageSource = ImageSource.FromStream(() => fileStream);

            CartaSingleton.Instance.NuevaCarta.FotoResult = file;

            //prueba
            //await DataBaseControl.Prueba(fileStream);
        }


        private async void ContinuarAction(object obj)
        {
            if (ImageSource != null)
            {
                await _navigation.PushAsync(new MapaPage());
                return;
            }
            await Application.Current.MainPage.DisplayAlert("Atención", "Añade una foto", "Vale");
        }

        //private string FileResultIntoByteString(FileResult fr) 
        //{
        //    string path = fr.FullPath;
        //    var byteList = File.ReadAllBytes(path);
        //    return Encoding.Default.GetString(byteList);
        //}
    }
}
