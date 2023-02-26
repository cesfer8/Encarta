using Encarta.Models;
using Encarta.Services;
using Encarta.Utilidades;
using Encarta.Views.Pages;
using Encarta.Views.Pages.PopUps;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class FavoritosViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        public ICommand NuevaCartaCommand { get; }
        public ICommand CartaPulsadaCommand { get; }
        public ICommand FavoritoPulsadoCommand { get; }
        public ICommand QuitarFiltrosCommand { get; }
        //public ICommand FiltrarRestaurantesCommand { get; } no funciona el control con el command

        private ObservableCollection<CartaVisor> _listadoCartas;
        public ObservableCollection<CartaVisor> ListadoCartas
        {
            get => _listadoCartas;
            set
            {
                SetProperty(ref _listadoCartas, value);
            }
        }

        private bool _isFiltered;
        public bool IsFiltered
        {
            get => _isFiltered;
            set
            {
                SetProperty(ref _isFiltered, value);
            }
        }

        private string _textoFiltro;
        public string TextoFiltro
        {
            get => _textoFiltro;
            set
            {
                SetProperty(ref _textoFiltro, value);
            }
        }

        public FavoritosViewModel(INavigation navigation)
        {
            _navigation = navigation;
            CartaPulsadaCommand = new Command<CartaVisor>(CartaPulsadaAction);
            NuevaCartaCommand = new Command(NuevaCartaAction);
            FavoritoPulsadoCommand = new Command<CartaVisor>(FavoritoPulsadoAction);
            QuitarFiltrosCommand = new Command(QuitarFiltrosAction);

            ListadoCartas = new ObservableCollection<CartaVisor>();
        }

        public async void FiltrarRestaurantes()
        {
            IsBusy = true;
            ObservableCollection<CartaVisor> nuevaLista = new ObservableCollection<CartaVisor>();
            foreach (CartaVisor c in await GetCartas())
            {
                if (c.Restaurante.Nombre.Contains(TextoFiltro.ToUpper()))
                {
                    nuevaLista.Add(c);
                }
            }
            ListadoCartas = nuevaLista;
            IsFiltered = true;
            IsBusy = false;
        }

        private async void QuitarFiltrosAction(object obj)
        {
            IsFiltered = false;
            await OnStart();
        }

        private async void FavoritoPulsadoAction(CartaVisor obj)
        {
            //IsBusy = true;
            ListadoCartas.Remove(obj);
            DataBaseControl.RemoveCartaFavUsuario(Preferences.Get("id_usuario", "none"), obj.Id);
            //IsBusy = false;
        }

        private async void NuevaCartaAction(object obj)
        {
            CartaSingleton.Instance.Clear();
            await _navigation.PushPopupAsync(new FormatoCartaPopUp());
        }

        private async void CartaPulsadaAction(CartaVisor obj)
        {
            await _navigation.PushAsync(new VerCartaPage(obj));
        }

        public async Task<ObservableCollection<CartaVisor>> GetCartas()
        {
            IsBusy = true;

            List<Carta> listaCartas = await DataBaseControl.GetCartasFav(Preferences.Get("id_usuario", "none"));
            listaCartas = listaCartas.OrderByDescending(x => x.Fecha_creacion).ToList();//ordenamos, mas nuevos primero
            bool esQr;
            ObservableCollection<CartaVisor> lista = new ObservableCollection<CartaVisor>();
            foreach (Carta c in listaCartas)
            {
                Restaurante r = await DataBaseControl.GetRestaurantesFromId(c.Id_restaurante);
                esQr = !String.IsNullOrEmpty(c.UrlQR);

                lista.Add(new CartaVisor
                {
                    Id = c.Id,
                    Foto = c.Foto,
                    UrlQR = c.UrlQR,
                    id_usuario = c.Id_usuario,
                    IsQR = esQr,
                    IsFav = true,
                    Fecha_creacion = c.Fecha_creacion,
                    Restaurante = r
                });
            }
            IsBusy = false;
            return lista;


        }

        public async Task OnStart()
        {
            if (!IsFiltered)
            {
                ListadoCartas = await GetCartas();
                return;
            }
            FiltrarRestaurantes();
        }
    }
}
