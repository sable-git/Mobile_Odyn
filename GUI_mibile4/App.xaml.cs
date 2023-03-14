using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RuleModule_Odyn;
using System.Collections.ObjectModel;


namespace GUI_mobile4
{
    public partial class App : Application
    {
        //readonly RuleModule rules;
        public static double ScreenWidth;
        public static double ScreenHeight;
        public static NavigationPage myNavigation;
        public App(RuleModule rules)
        {
            Xamarin.Forms.DataGrid.DataGridComponent.Init();
            InitializeComponent();            
          //  this.rules = rules;
            
            TPage tp=new TPage(rules);            
            //App.myNavigation.Navigation.PushAsync(tp);
            //NavigationPage.SetHasBackButton(this, false);
            //NavigationPage.SetHasNavigationBar(this, false);
            //MainPage = nav;
            MainPage = tp;
           

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
