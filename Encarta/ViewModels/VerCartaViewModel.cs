using Encarta.Models;
using Encarta.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class VerCartaViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        public ICommand EnlaceQrCommand { get; }
        public ICommand ChangeFavCommand { get; }
        public ICommand BorrarCartaCommand { get; }
        public ICommand AbrirMapsCommand { get; }
        public ICommand DenunciarCartaCommand { get; }
        public ICommand QuitarDenunciasCommand { get; }
        public ICommand BloquearUsuarioCommand { get; }


        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private CartaVisor _carta;
        public CartaVisor Carta
        {
            get => _carta;
            set
            {
                SetProperty(ref _carta, value);
                LoadData();
            }
        }

        private bool _isFavorito;
        public bool IsFavorito
        {
            get => _isFavorito;
            set
            {
                SetProperty(ref _isFavorito, value);
            }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                SetProperty(ref _isAdmin, value);
            }
        }

        private bool _esMiCarta = false;
        public bool EsMiCarta
        {
            get => _esMiCarta;
            set
            {
                SetProperty(ref _esMiCarta, value);
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

        //public ICommand ContinuarCommand { get; }
        public VerCartaViewModel(INavigation navigation, CartaVisor carta, bool IsAdmin)
        {
            _navigation = navigation;
            this.Carta = carta;
            this.IsAdmin = IsAdmin;
            EnlaceQrCommand = new Command(async () => await EnlaceQrAction());
            ChangeFavCommand = new Command(ChangeFavAction);
            BorrarCartaCommand = new Command(async () => await BorrarCartaAction());
            DenunciarCartaCommand = new Command(async () => await DenunciarCartaAction());
            AbrirMapsCommand = new Command(async () => await AbrirMapsAction());
            QuitarDenunciasCommand = new Command(async () => await QuitarDenunciasAction());
            BloquearUsuarioCommand = new Command(async () => await BloquearUsuarioAction());
        }

        private async Task BloquearUsuarioAction()
        {
            bool bloquear = await Application.Current.MainPage.DisplayAlert("Atención", "El usuario será bloqueado de forma permanente", "Si, bloquear", "Cancelar");
            if (!bloquear)
            {
                return;
            }
            IsBusy = true;
            bool isBloqueado = await DataBaseControl.BlockUsuario(Carta.id_usuario);
            IsBusy = false;
            if (isBloqueado)
            {
                bool eliminarCarta = await Application.Current.MainPage.DisplayAlert("", "Usuario bloqueado. ¿Eliminar la carta?", "Si, eliminar carta", "No");
                if (eliminarCarta)
                {
                    IsBusy = true;
                    await DataBaseControl.RemoveCarta(Carta.Id);
                    IsBusy = false;
                    await Application.Current.MainPage.DisplayAlert("", "Carta borrada", "Vale");
                    await _navigation.PopAsync();
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("", "Ha habido un error", "Vale");
            }
        }

        private async Task QuitarDenunciasAction()
        {
            bool quitarDenuncias = await Application.Current.MainPage.DisplayAlert("Atención", "La carta será exenta de denuncias", "Quitar denuncias", "Cancelar");
            if (!quitarDenuncias)
            {
                return;
            }
            IsBusy = true;
            quitarDenuncias = await DataBaseControl.RemoveDenunciasCarta(Carta.Id);
            IsBusy = false;
            if (quitarDenuncias)
            {
                await Application.Current.MainPage.DisplayAlert("", "¡Listo! Carta sin denuncias", "Vale");
                await _navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("", "Ha habido un error", "Vale");
            }
        }

        private async Task DenunciarCartaAction()
        {
            bool denunciar = await Application.Current.MainPage.DisplayAlert("Atención", "¿Estás seguro que la carta incumple las normas de Encarta?", "Si, denunciar", "Cancelar");
            if (!denunciar)
            {
                return;
            }

            IsBusy = true;
            bool denunciado = await DataBaseControl.AddDenunciaCarta(Carta.Id);
            IsBusy = false;

            if (denunciado)
            {
                await Application.Current.MainPage.DisplayAlert("", "Carta denunciada, pronto un administrador revisara la incidencia. ¡Gracias por hacer de Encarta un lugar mejor!", "Vale");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("", "Ha habido un error", "Vale");
            }

            await _navigation.PopAsync();
        }

        private async Task AbrirMapsAction()
        {
            try
            {
                var location = new Location(Carta.Restaurante.Latitude, Carta.Restaurante.Longitude);
                var options = new MapLaunchOptions { Name = Carta.Restaurante.Nombre };
                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private async Task BorrarCartaAction()
        {
            bool borrarCarta = await Application.Current.MainPage.DisplayAlert("Atención", "Se borrará la carta", "Borrar", "Cancelar");
            if (!borrarCarta)
            {
                return;
            }
            IsBusy = true;
            await DataBaseControl.RemoveCarta(Carta.Id);
            IsBusy = false;
            await Application.Current.MainPage.DisplayAlert("", "Carta borrada", "Vale");
            await _navigation.PopAsync();
        }

        private void ChangeFavAction(object obj)
        {
            Carta.IsFav = !Carta.IsFav;
            IsFavorito = Carta.IsFav; //Fix bug, no funciona poner Carta.IsFav en xaml
            if (Carta.IsFav)
            {
                DataBaseControl.AddCartaFavUsuario(Preferences.Get("id_usuario", "none"), Carta.Id);
                return;
            }
            DataBaseControl.RemoveCartaFavUsuario(Preferences.Get("id_usuario", "none"), Carta.Id);
        }

        private async Task EnlaceQrAction()
        {
            try
            {
                await Launcher.OpenAsync(Carta.UrlQR);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private async void LoadData()
        {
            Title = Carta.Restaurante.Nombre;
            IsFavorito = Carta.IsFav;

            //List<Carta> listadoCartas = await DataBaseControl.GetCartasFromUsuario(Preferences.Get("id_usuario", "none"));
            //Carta c = listadoCartas.Where(x => x.Id.Equals(Carta.Id)).FirstOrDefault();


            if (Carta.id_usuario.Equals(Preferences.Get("id_usuario", "none")))
            {
                EsMiCarta = true;
            }

            if (!Carta.IsQR)
            {
                this.ImageSource = Carta.Foto;
            }
        }
    }
}