using Encarta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Encarta.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapaPage : ContentPage
    {

        private MapaViewModel vm;
        public MapaPage()
        {
            InitializeComponent();
            vm = new MapaViewModel(Navigation, map);
            BindingContext = vm;
            MoveMapToStartPosition(); 
        }

        private async void MoveMapToStartPosition()
        {
            Location location = await GetLocation();
            Position pos = new Position(location.Latitude, location.Longitude);
            MapSpan mapSpan = new MapSpan(pos, 1, 1);
            map.MoveToRegion(mapSpan.WithZoom(700));
        }

        private async Task<Location> GetLocation()
        {
            Location location = await Geolocation.GetLastKnownLocationAsync();
            if(location != null)
            {
                return location;
            }
            return new Location(43.34, -4.04);
        }

        private void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            vm.MapClicked(e);
        }

        protected override bool OnBackButtonPressed()
        {
            vm.BackButtonAction(new Object());
            return true;
        }
    }
}