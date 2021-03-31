using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Birds.Droid.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


#pragma warning disable CS0612 // Type or member is obsolete
[assembly: ExportRenderer(typeof(Xamarin.Forms.DatePicker), typeof(CustomDatePickerRenderer))]
#pragma warning restore CS0612 // Type or member is obsolete
namespace Birds.Droid.Renderers
{
    [Obsolete]
    public class CustomDatePickerRenderer : Xamarin.Forms.Platform.Android.DatePickerRenderer
    {
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            this.Control.SetTextColor(Android.Graphics.Color.Black);
            Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);

        }
    }
}