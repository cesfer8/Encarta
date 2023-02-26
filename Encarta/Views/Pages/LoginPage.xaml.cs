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
    public partial class LoginPage : ContentPage
    {
        LoginViewModel vm;
        public LoginPage()
        {
            InitializeComponent();
            vm = new LoginViewModel(Navigation);
            BindingContext = vm;
        }

        protected override bool OnBackButtonPressed()
        {
            string hola = "";
            return false;
        }

        protected override void OnAppearing()
        {
            vm.PageAppearing();
            base.OnAppearing();
        }
    }
}