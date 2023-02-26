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
    public partial class FotoCartaPage : ContentPage
    {
        private FotoCartaViewModel vm;
        public FotoCartaPage()
        {
            InitializeComponent();
            vm = new FotoCartaViewModel(Navigation);
            this.BindingContext = vm;
        }
    }
}