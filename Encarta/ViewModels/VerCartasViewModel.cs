using Encarta.Models;
using Encarta.Services;
using Encarta.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Encarta.ViewModels
{
    public class VerCartasViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        public ICommand FramePulsadoCommand { get; }

        private ObservableCollection<CartaVisor> _listadoCartas;
        public ObservableCollection<CartaVisor> ListadoCartas
        {
            get => _listadoCartas;
            set
            {
                SetProperty(ref _listadoCartas, value);
            }
        }

        public VerCartasViewModel(INavigation navigation, Restaurante r)
        {
            _navigation = navigation;
            FramePulsadoCommand = new Command<CartaVisor>(FramePulsadoAction);
            Title = r.Nombre;
            GetCartas(r.Id);
        }

        private async void FramePulsadoAction(CartaVisor obj)
        {
            await _navigation.PushAsync(new VerCartaPage(obj));
        }

        private async void GetCartas(string idRestaurante)
        {
            IsBusy = true;

            List<Carta> listaCartas = await DataBaseControl.GetCartasFromRestaurante(idRestaurante);
            listaCartas = listaCartas.OrderByDescending(x => x.Fecha_creacion).ToList();//ordenamos, mas nuevos primero
            bool esQr, esFav;
            ObservableCollection<CartaVisor> lista = new ObservableCollection<CartaVisor>();
            List<string> idsCartasFav = (await DataBaseControl.GetUsuario(Preferences.Get("id_usuario", "none"))).CartasFavoritas;

            foreach (Carta c in listaCartas)
            {
                Restaurante r = await DataBaseControl.GetRestaurantesFromId(c.Id_restaurante);
                esQr = !String.IsNullOrEmpty(c.UrlQR);
                if (idsCartasFav == null)
                {
                    esFav = false;
                }
                else
                {
                    esFav = idsCartasFav.Contains(c.Id);
                }
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
            ListadoCartas = lista;
            IsBusy = false;

        }
    }
}
