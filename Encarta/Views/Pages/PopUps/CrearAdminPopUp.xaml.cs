using Encarta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Encarta.Views.Pages.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CrearAdminPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        CrearAdminViewModel vm;
        public CrearAdminPopUp(INavigation navigation)
        {
            InitializeComponent();
            vm = new CrearAdminViewModel(navigation);
            BindingContext = vm;
        }
    }
}