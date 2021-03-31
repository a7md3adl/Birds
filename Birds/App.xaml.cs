using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Birds.Services;
using Birds.Views;
using System.IO;

namespace Birds
{
    public partial class App : Application
    {
        public static string TempImagePath {set;get;}
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            if(TempImagePath != null)
            {
                if (File.Exists(TempImagePath)) File.Delete(TempImagePath);
            }
        }
    }
}
