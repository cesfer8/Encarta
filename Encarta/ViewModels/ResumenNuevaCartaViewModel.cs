using Encarta.Models;
using Encarta.Services;
using Encarta.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Encarta.ViewModels
{
    public class ResumenNuevaCartaViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private Pin _pin;
        private bool hasRestaurante;
        public ICommand EnlaceQrCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand ChangeFavCommand { get; }

        private string _nombreRestaurante;
        public string NombreRestaurante
        {
            get => _nombreRestaurante;
            set
            {
                SetProperty(ref _nombreRestaurante, value);
            }
        }

        private string _urlQR;
        public string UrlQR
        {
            get => _urlQR;
            set
            {
                SetProperty(ref _urlQR, value);
            }
        }

        private bool _entryEnable = true;
        public bool EntryEnable
        {
            get => _entryEnable;
            set
            {
                SetProperty(ref _entryEnable, value);
            }
        }

        private bool _isFavorito = true;
        public bool IsFavorito
        {
            get => _isFavorito;
            set
            {
                SetProperty(ref _isFavorito, value);
            }
        }

        private bool _isQR;
        public bool IsQR
        {
            get => _isQR;
            set
            {
                SetProperty(ref _isQR, value);
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

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        public ResumenNuevaCartaViewModel(INavigation navigation, Pin pin)
        {
            _navigation = navigation;
            _pin = pin;
            EnlaceQrCommand = new Command(async () => await EnlaceQrAction());
            GuardarCommand = new Command(async () => await GuardarAction());
            ChangeFavCommand = new Command(ChangeFavAction);
            hasRestaurante = CartaSingleton.Instance.Restaurante != null;
            InicializarFrames();
        }

        private void ChangeFavAction(object obj)
        {
            IsFavorito = !IsFavorito;
        }

        private async Task GuardarAction()
        {

            if (NombreError = String.IsNullOrEmpty(NombreRestaurante) || NombreRestaurante.Length <= 1)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Añade el nombre del restaurante", "Vale");
                return;
            }

            IsBusy = true;
            bool guardado = await Guardar();
            IsBusy = false;
            if (guardado)
            {
                await Application.Current.MainPage.DisplayAlert("", "¡Carta guardada!", "Vale");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("", "Error: La carta no se ha podido guardar", "Vale");
            }
            Salir();
        }

        private async Task<bool> Guardar()
        {

            Restaurante restaurante = new Restaurante();
            if (hasRestaurante)
            {
                restaurante = CartaSingleton.Instance.Restaurante;
            }
            else
            {
                restaurante = new Restaurante
                {
                    Nombre = NombreRestaurante.ToUpper(),
                    Latitude = _pin.Position.Latitude,
                    Longitude = _pin.Position.Longitude,
                    Id_usuario = Preferences.Get("id_usuario", "none")
                };
            }

            Carta nuevaCarta = CartaSingleton.Instance.NuevaCarta;

            if (!IsQR)
            {
                Stream imgStream = await CartaSingleton.Instance.NuevaCarta.FotoResult.OpenReadAsync();
                nuevaCarta.Foto = await DataBaseControl.AddCartaImagen(imgStream, (imgStream as FileStream).Name);
                nuevaCarta.FotoResult = null;
                //nuevaCarta.Foto = Convert.ToBase64String(File.ReadAllBytes(nuevaCarta.Foto));
            }
            nuevaCarta.Fecha_creacion = DateTime.Now;
            nuevaCarta.Id_usuario = Preferences.Get("id_usuario", "none");
            string idCarta = String.Empty;
            bool cartaAniadida = false;
            if (hasRestaurante)
            {
                nuevaCarta.Id_restaurante = restaurante.Id;

                if ((idCarta = await DataBaseControl.AddCartaToRestaurante(nuevaCarta)) != String.Empty)
                {
                    cartaAniadida = true;
                }
            }
            else
            {
                if ((idCarta = await DataBaseControl.AddRestaurante(restaurante, nuevaCarta)) != String.Empty)
                {
                    cartaAniadida = true;
                }
            }

            if (cartaAniadida) //Añadimos carta a favoritos
            {
                if (IsFavorito)
                {
                    await DataBaseControl.AddCartaFavUsuario(Preferences.Get("id_usuario", "none"), idCarta);
                }
                return true;
            }

            return false;
        }

        private void Salir()
        {
            var existingPages = _navigation.NavigationStack.ToList();
            foreach (var page in existingPages)
            {
                if (page != null)
                    _navigation.RemovePage(page);
            }
        }

        private async Task EnlaceQrAction()
        {
            try
            {
                await Launcher.OpenAsync(CartaSingleton.Instance.NuevaCarta.UrlQR);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void InicializarFrames()
        {
            if (hasRestaurante)
            {
                NombreRestaurante = CartaSingleton.Instance.Restaurante.Nombre;
                EntryEnable = false;
            }

            if (CartaSingleton.Instance.NuevaCarta.FotoResult == null)
            {
                IsQR = true;
                UrlQR = CartaSingleton.Instance.NuevaCarta.UrlQR;
            }
            else
            {
                IsQR = false;
                //ImageSource.FromStream(() => fileStream);
                Stream fotoStream = await CartaSingleton.Instance.NuevaCarta.FotoResult.OpenReadAsync();
                ImageSource = ImageSource.FromStream(() => fotoStream);
            }
        }

        private string FileResultIntoByteString(FileResult fr)
        {
            string path = fr.FullPath;
            var byteList = File.ReadAllBytes(path);
            return Encoding.Default.GetString(byteList);
        }
    }
}
