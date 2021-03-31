using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Birds.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;


using Birds.Droid;
using System.IO;
using System.Threading.Tasks;
using Android.Content.PM;

[assembly: Dependency(typeof(PlatformSpecificAndroid))]
[assembly: Dependency(typeof(Restart))]
[assembly: Dependency(typeof(FileService))]
[assembly: Dependency(typeof(OpenAppAndroid))]
namespace Birds.Droid
{
    public class Restart : IRestart
    {
       

        void IRestart.Restart()
        {
            MainActivity.MA.FinishAffinity();
            DependencyService.Get<IAppHandler>().LaunchApp("com.alexandrinasoftware.birds");
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            //MainActivity.MA.Recreate();

            // Intent intent = new Intent(Android.App.Application.Context, typeof(MainActivity));
            //MainActivity.MA.StartActivity(intent);
            //MainActivity.MA.FinishAffinity();
            //Intent i = MainActivity.MA.BaseContext.PackageManager
            // .GetLaunchIntentForPackage(MainActivity.MA.BaseContext.PackageName);
            //i.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            //MainActivity.MA.StartActivity(i);
        }
    }
    public class PlatformSpecificAndroid : IPlatformSpecificAndroid
    {
        public void DisplayInGallery(string filePath)
        {
            OpenImageInDefaultViewer(filePath);
        }
        #region Members Definition
        #endregion

        #region Properties interface
        #endregion

        #region Constructor / Destructor
        #endregion

        #region Public Functions
        public void OpenImageInDefaultViewer(string p_strImageFilePath)
        {
            Intent l_iIntent = new Intent();

            l_iIntent.SetAction(Intent.ActionView);

            l_iIntent.SetFlags(ActivityFlags.NewTask);
            l_iIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
            l_iIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);

            var l_fImage = new Java.IO.File(p_strImageFilePath);

            var l_uFileUri = Android.Support.V4.Content.FileProvider.GetUriForFile(
              Android.App.Application.Context, "com.alexandrinasoftware.birds.fileprovider", l_fImage);

            l_iIntent.SetDataAndType(l_uFileUri, "image/*");
            //l_iIntent.SetDataAndType(Android.Net.Uri.FromFile(l_fImage), "image/*");

            Android.App.Application.Context.StartActivity(l_iIntent);
        }
        #endregion

        #region Private Functions
        #endregion
    }

    public class FileService : IFileService
    {
        [Obsolete]
        public void SavePicture(string name, Stream data, string location = "temp")
        {
            var documentsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            documentsPath = Path.Combine(documentsPath);
            Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, name);

            byte[] bArray = new byte[data.Length];
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (data)
                {
                    data.Read(bArray, 0, (int)data.Length);
                }
                int length = bArray.Length;
                fs.Write(bArray, 0, length);
            }
        }
    }

    public class OpenAppAndroid : IAppHandler
    {
        public Task<bool> LaunchApp(string packageName)
        {


            bool result = false;

            try
            {

                PackageManager pm = Android.App.Application.Context.PackageManager;

                if (IsAppInstalled(packageName))
                {

                    Intent intent = pm.GetLaunchIntentForPackage(packageName);
                    if (intent != null)
                    {

                        intent.SetFlags(ActivityFlags.NewTask);
                        Android.App.Application.Context.StartActivity(intent);
                    }
                }

                else
                {

                    Intent intent = pm.GetLaunchIntentForPackage("the package name of play store on your device");
                    if (intent != null)
                    {

                        intent.SetFlags(ActivityFlags.NewTask);
                        Android.App.Application.Context.StartActivity(intent);
                    }
                }
            }
            catch (ActivityNotFoundException)
            {
                result = false;
            }

            return Task.FromResult(result);
        }


        public bool IsAppInstalled(string packageName)
        {
            PackageManager pm = Android.App.Application.Context.PackageManager;
            bool installed = false;
            try
            {
                pm.GetPackageInfo(packageName, PackageInfoFlags.Activities);
                installed = true;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                installed = false;
            }

            return installed;
        }

    }
}

