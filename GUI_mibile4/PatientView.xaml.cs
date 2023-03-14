using RuleModule_Odyn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GUI_mobile4
{

   

    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PatientTools : ContentPage
    {
        readonly ObservableCollection<Patient> patients;
        public int FontSize { get; set; }
        readonly TPage t;
        public PatientTools(ObservableCollection<Patient> _patients,TPage _t)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            patients = _patients;
            t = _t;
            FontSize = StaticElements.fontSize;
            InitializeComponent();
            patients.CollectionChanged += Patients_CollectionChanged;
            pList.ItemsSource = patients;                        
            BindingContext = this;
            //pList.lab.FontSize = 30;
            pList.ItemTapped += (sender, e) => {               
                 t.ActivePatient = (Patient)e.Item;
            };
         
            

            
        }
        async private void Patients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (patients.Count > 0)
            {
                await Task.Delay(100);
                pList.SelectedItem = patients[patients.Count - 1];
            }
        }
        async private void Button_Clicked(object sender, EventArgs e)
        {
            NewPatient p = new NewPatient(t);
            //await App.Current.MainPage.Navigation.PushAsync(p);
            await this.Navigation.PushAsync(p);
            
            /*
            PatientInfo pi = new PatientInfo();
            pi.Id = "Test";
            t.AddNewPatient(pi);
            pList.SelectedItem = patients[patients.Count - 1];            */

        }
        private void Reset_Clicked(object sender, EventArgs e)
        {
            t.ResetPages();
        }
        private void Delete_Clicked(object sender, EventArgs e)
        {
            patients.Remove(patients.LastOrDefault()); 
            if(patients.Count==0)
            {
                t.RemoveChildren();
            }
        }


    }
}