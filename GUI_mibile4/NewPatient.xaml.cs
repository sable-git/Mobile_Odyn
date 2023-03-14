using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GUI_mobile4
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPatient : ContentPage
    {
        readonly TPage t;
        bool press = false;
        readonly int size;
        public NewPatient(TPage t)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            this.t = t;

            size = StaticElements.fontSize;
            InitializeComponent();
            id.FontSize = size;
            button.FontSize = size;
            labelT.FontSize = size;
            button.Clicked += Button_Clicked;

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (!press)
            {
                press = true;
                PatientInfo pi = new PatientInfo()
                {
                    Id = id.Text,
                    FontSize = StaticElements.fontSize
                };
                t.AddNewPatient(pi);
                this.Navigation.PopAsync();
                //App.myNavigation.PopAsync();
            }
        }
    }
}