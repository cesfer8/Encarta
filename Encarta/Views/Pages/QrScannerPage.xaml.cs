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
    public partial class QrScannerPage : ContentPage
    {
        private QrScannerViewModel vm;
        public QrScannerPage()
        {
            InitializeComponent();
            vm = new QrScannerViewModel(Navigation);
            BindingContext = vm;
        }
    }
}