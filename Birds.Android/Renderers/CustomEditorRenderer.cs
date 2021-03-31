using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Birds.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

#pragma warning disable CS0612 // Type or member is obsolete
[assembly: ExportRenderer(typeof(Editor), typeof(CustomEditorRenderer))]
#pragma warning restore CS0612 // Type or member is obsolete

namespace Birds.Droid
{
    [Obsolete]
    public class CustomEditorRenderer : EditorRenderer
    {
        //public CustomEditorRenderer(Context context) : base(context)
        //{
        //}

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                this.Control.SetBackgroundDrawable(gd);
                this.Control.SetRawInputType(InputTypes.TextFlagImeMultiLine | InputTypes.TextFlagMultiLine | InputTypes.TextVariationLongMessage | InputTypes.ClassText);
                //this.Control.GetInputExtras(true);
                this.Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.LightGray));
            }
        }
         

    }
}