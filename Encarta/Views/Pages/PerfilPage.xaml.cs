using Encarta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Encarta.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PerfilPage : ContentPage
    {
        PerfilViewModel vm;
        public PerfilPage()
        {
            InitializeComponent();
            vm = new PerfilViewModel(Navigation);
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            vm.PageAppearing();
            base.OnAppearing();
        }
    }
}