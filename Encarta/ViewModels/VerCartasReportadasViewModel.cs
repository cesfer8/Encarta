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
    public class VerCartasReportadasViewModel : BaseViewModel
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

        public VerCartasReportadasViewModel(INavigation navigation)
        {
            _navigation = navigation;
            FramePulsadoCommand = new Command<CartaVisor>(FramePulsadoAction);
        }

        private async void FramePulsadoAction(CartaVisor obj)
        {
            await _navigation.PushAsync(new VerCartaPage(obj,true));
        }

        public void OnAppearing()
        {
            GetCartas();
        }

        private async void GetCartas()
        {
            IsBusy = true;

            List<Carta> listaCartas = await DataBaseControl.GetCartasWithDenuncias();
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
                    Restaurante = r,
                    Denuncias = c.Denuncias
                });
            }
            ListadoCartas = lista;
            IsBusy = false;

        }
    }
}
