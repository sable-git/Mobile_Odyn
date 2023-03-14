using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RuleModule_Odyn;

namespace GUI_mobile
{
    public partial class App : Application
    {
        RuleModule rules;
        public App(RuleModule rules)
        {
            InitializeComponent();
            this.rules = rules;
            MainPage = new NavigationPage(new MainPage(rules));
            
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
