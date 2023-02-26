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
    public partial class MisCartasPage : ContentPage
    {
        MisCartasViewModel vm;
        public MisCartasPage()
        {
            InitializeComponent();
            vm = new MisCartasViewModel(Navigation);
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            vm.OnStart();
            base.OnAppearing();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Frame frame = (Frame)sender;
            var scaleUp = frame.ScaleTo(0.95, 50);
            var fadeOut = frame.FadeTo(0.8, 70);
            await Task.WhenAll(fadeOut, scaleUp);
            frame.ScaleTo(1, 70);
            frame.FadeTo(1, 70);
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            vm.FiltrarRestaurantes();
        }

        private void TapGestureRecognizer_RemoveFilter(object sender, EventArgs e)
        {
            searchEntry.Text = String.Empty;
        }
    }
}