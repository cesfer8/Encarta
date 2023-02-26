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
    public partial class HomePage : ContentPage
    {
        HomeViewModel vm;
        public HomePage()
        {
            InitializeComponent();
            vm = new HomeViewModel(Navigation);
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            vm.OnAppearing();
            base.OnAppearing();
        }

        private async void frame_Tapped(object sender, EventArgs e)
        {
            Frame frame = sender as Frame;
            var scaleUp = frame.ScaleTo(0.95, 50);
            var fadeOut = frame.FadeTo(0.8, 70);
            await Task.WhenAll(fadeOut, scaleUp);
            frame.ScaleTo(1, 70);
            frame.FadeTo(1, 70);
        }
    }
}