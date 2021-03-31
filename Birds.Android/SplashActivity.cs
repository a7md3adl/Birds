using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace Birds.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]

    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
                Window.DecorView.SetFitsSystemWindows(true);

                Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                //Window.DecorView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.SplashLAyOut);
            

        }

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {

            //base.OnCreate(savedInstanceState, persistentState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            //global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //SetContentView(Resource.Layout.SplashLAyOut);
            //FindViewById<TextView>(Resource.Id.txtAppVersion).Text = $"Version {PackageManager.GetPackageInfo(PackageName, 0).VersionName}";

        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            await Task.Delay(1); // Simulate a bit of startup work.

            RunOnUiThread(() =>
            {

                FindViewById<TextView>(Resource.Id.txtAppVersion).Text = $"Version {PackageManager.GetPackageInfo(PackageName, 0).VersionName}";


                Task.Delay(1000); // Simulate a bit of startup work.
                                       // await ConnectToServers();


                StartActivity(new Intent(Android.App.Application.Context, typeof(MainActivity)));
            });
        }

    }
}