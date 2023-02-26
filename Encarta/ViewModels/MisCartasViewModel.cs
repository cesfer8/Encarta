using Acr.UserDialogs;
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
    public class MisCartasViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        public ICommand NuevaCartaCommand { get; }
        public ICommand CartaPulsadaCommand { get; }
        public ICommand QuitarFiltrosCommand { get; }
        public ICommand FavoritoPulsadoCommand { get; }

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

        public MisCartasViewModel(INavigation navigation)
        {
            _navigation = navigation;
            CartaPulsadaCommand = new Command<CartaVisor>(CartaPulsadaAction);
            NuevaCartaCommand = new Command(NuevaCartaAction);
            QuitarFiltrosCommand = new Command(QuitarFiltrosAction);
            FavoritoPulsadoCommand = new Command<CartaVisor>(FavoritoPulsadoAction);

            ListadoCartas = new ObservableCollection<CartaVisor>();
        }

        private async void FavoritoPulsadoAction(CartaVisor obj)
        {
            //IsBusy = true;
            ObservableCollection<CartaVisor> nuevaLista = new ObservableCollection<CartaVisor>();
            foreach(CartaVisor c in ListadoCartas)
            {
                if (c.Id.Equals(obj.Id))
                {
                    c.IsFav = !c.IsFav;
                }
                nuevaLista.Add(c);
            }
            ListadoCartas = nuevaLista;
            
            if (obj.IsFav)
            {
                DataBaseControl.AddCartaFavUsuario(Preferences.Get("id_usuario", "none"), obj.Id);
            }
            else
            {
                DataBaseControl.RemoveCartaFavUsuario(Preferences.Get("id_usuario", "none"), obj.Id);
            }
            //IsBusy = false;
        }

        private async void QuitarFiltrosAction(object obj)
        {
            IsFiltered = false;
            await OnStart();
        }

        public async void FiltrarRestaurantes()
        {
            IsBusy = true;
            ObservableCollection<CartaVisor> nuevaLista = new ObservableCollection<CartaVisor>();
            foreach(CartaVisor c in await GetCartas())
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
            
            List<Carta> listaCartas = await DataBaseControl.GetCartasFromUsuario(Preferences.Get("id_usuario", "none"));
            listaCartas = listaCartas.OrderByDescending(x => x.Fecha_creacion).ToList();//ordenamos, mas nuevos primero
            bool esQr, esFav;
            ObservableCollection<CartaVisor> lista = new ObservableCollection<CartaVisor>();
            List<string> idsCartasFav =  (await DataBaseControl.GetUsuario(Preferences.Get("id_usuario", "none"))).CartasFavoritas;

            foreach (Carta c in listaCartas)
            {
                Restaurante r = await DataBaseControl.GetRestaurantesFromId(c.Id_restaurante);
                esQr = !String.IsNullOrEmpty(c.UrlQR);
                esFav = idsCartasFav.Contains(c.Id);
                lista.Add(new CartaVisor
                {
                    Id = c.Id,
                    Foto = c.Foto,
                    UrlQR = c.UrlQR,
                    IsQR = esQr,
                    IsFav = esFav,
                    id_usuario = c.Id_usuario,
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
