using Encarta.Models;
using Encarta.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Encarta.Views.Pages.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantesCercaPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        RestaurantesCercaPopUpViewModel vm;
        public RestaurantesCercaPopUp(INavigation n, ObservableCollection<Restaurante> listaRestaurantes, Xamarin.Forms.Maps.Pin pin)
        {
            InitializeComponent();
            vm = new RestaurantesCercaPopUpViewModel(n, listaRestaurantes, pin);
            BindingContext = vm;

            //await Navigation.PopPopupAsync();
        }
    }
}