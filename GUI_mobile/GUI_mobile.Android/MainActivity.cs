using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.IO;
using Android.Content.Res;
using RuleModule_Odyn;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Xamarin.Forms;
using GUI_mobile4;

namespace GUI_mobile.Droid
{
    

    [Activity(Label = "GUI_mobile4", LaunchMode = LaunchMode.SingleTop,Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape,ConfigurationChanges = ConfigChanges.ScreenSize |  ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            AssetManager asset = this.Assets;
            RuleModule rules = new RuleModule();
            Stream st;// = asset.Open("inicjalizacjaVC-OS-c6.txt");
            //rules.AddSchema("INIT", st);
            st = asset.Open("SPO2_PaO2_final_O.txt");
            rules.AddSchema("Grudziadz", st);
            var width = Resources.DisplayMetrics.WidthPixels;
            var height = Resources.DisplayMetrics.HeightPixels;
//            var density = Resources.DisplayMetrics.Density;

            App.ScreenWidth = width;//(width - 0.5f) / density;
            App.ScreenHeight = height;// (height - 0.5f) / density;
                        

            LoadApplication(new App(rules));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public Stream GetInitial()
        {
            AssetManager asset = this.Assets;
            return asset.Open("inicjalizacjaVC-OS-c6.txt");
        }
    }
}