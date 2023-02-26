using Encarta.Models;
using Encarta.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Encarta.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerCartasPage : ContentPage
    {
        VerCartasViewModel vm;
        public VerCartasPage(Restaurante r)
        {
            InitializeComponent();
            vm = new VerCartasViewModel(Navigation, r);
            BindingContext = vm;
        }
    }
}