
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android.AppCompat;
using GUI_mobile4;
using Xamarin.Forms;
using Google.Android.Material.Tabs;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(TPage), typeof(GUI_mobile.Droid.Renderer))]
namespace GUI_mobile.Droid
{
    
    
    class Renderer : TabbedPageRenderer
    {
        private TabLayout tabLayout = null;
        readonly Context c;
        public Renderer(Context context):base(context)
        {
            c = context;
        }
        /*protected override void SetTabIcon(TabLayout.Tab tab, FileImageSource icon)
        {
            base.SetTabIcon(tab, icon);

            Android.Views.View v = new Android.Views.View(c);
            v.LayoutParameters = new LayoutParams(500, 50);
            tab.SetCustomView(v);
        }*/
        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            this.tabLayout = (TabLayout)this.GetChildAt(1);
            ChangeTabsFont();
        }

        private void ChangeTabsFont()
        {
            //Typeface font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "fonts/" + Constants.FontStyle);
            ViewGroup vg = (ViewGroup)tabLayout.GetChildAt(0);
            int tabsCount = vg.ChildCount;
            for (int j = 0; j < tabsCount; j++)
            {
                ViewGroup vgTab = (ViewGroup)vg.GetChildAt(j);
                int tabChildsCount = vgTab.ChildCount;
                for (int i = 0; i < tabChildsCount; i++)
                {
                    Android.Views.View tabViewChild = vgTab.GetChildAt(i);
                    if (tabViewChild is TextView t)
                    {
                        //((TextView)tabViewChild).Typeface = font;
                        t.TextSize = 15;

                    }
                }
            }
        }
    }
}