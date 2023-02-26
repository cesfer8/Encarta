using Encarta.Services;
using Encarta.Views;
using Encarta.Views.Pages;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("fontello.ttf")]
namespace Encarta
{
    public partial class App : Application
    {
        readonly public static string APIKEY = "AIzaSyCI4_wqSE37mNhnUciD9nWUt6UHUiDnmYI";
        readonly public static string DBURL = "https://encartafirebase-default-rtdb.europe-west1.firebasedatabase.app/";
        readonly public static string DBSTORAGE = "encartafirebase.appspot.com";

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense("Njc3MzM2QDMyMzAyZTMyMmUzMGgrKzVtZzdaei9Ud1lBN0NKVmdCNm9iNTRzbVR2ak1UTE9BVFdYQnRQb2s9");
            
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
