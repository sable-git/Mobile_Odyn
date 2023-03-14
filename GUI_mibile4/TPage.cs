using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using RuleModule_Odyn;
using Xamarin.Forms;

namespace GUI_mobile4
{
    public class HistoryRecord
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string TiTe { get; set; }
        public string RR { get; set; }
        public string Ti { get; set; }
        public string PEEP { get; set; }
        public string Ppeak { get; set; }
        public string Vt { get; set; }
        public string FiO2 { get; set; }
        public string SpO2 { get; set; }
        public string PaO2 { get; set; }
        public string PCO2 { get; set; }
        public string  PH { get; set; }


    }


    public class TPage:TabbedPage,INotifyPropertyChanged
    {
        readonly ObservableCollection<Patient> patients=new ObservableCollection<Patient>();
        private Patient _activePatient;
        public event PropertyChangedEventHandler PChanged;

        public Patient ActivePatient
        {
            get { return _activePatient; }
            set { _activePatient = value;SetActivePatient(_activePatient); OPropertyChanged();}
        }
        protected void OPropertyChanged([CallerMemberName] string name = null)
        {
            PChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        readonly RuleModule rulesClean;

        

        public TPage(RuleModule rules)
        {
            this.rulesClean = rules;            
            this.IconImageSource = null;
            
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
/*                                    NavigationPage navigationPage = new NavigationPage(new MainPage(rules,h));
                                    navigationPage.Title = "Sugestie";              
                                    Children.Add(navigationPage);
                                    NavigationPage navigationPage1 = new NavigationPage(new SchemaPage(rules));           
                                    navigationPage1.Title = "Aktywny schemat";
                                    Children.Add(navigationPage1);
                                    NavigationPage navigationPage2 = new NavigationPage(new HistoryPage(h));
                                    navigationPage2.Title = "Historia";
                                    Children.Add(navigationPage2);*/
            NavigationPage.SetHasNavigationBar(this, false);
           
            var p = new PatientTools(patients, this);
            Children.Add(new NavigationPage(p));            
            Children[0].Title = "Pacjenci";
            /*            Children.Add(new MainPage(rules, h));
                        Children[0].Title= "Sugestie";
                        Children.Add(new SchemaPage(rules));
                        Children[1].Title = "Aktywny schemat";
                        Children.Add(new HistoryPage(h));
                        Children[2].Title = "Historia";*/
            
            //this.CurrentPage= new NavigationPage(p);
        }

       

        public void AddNewPatient(PatientInfo id)
        {            
            RuleModule r =(RuleModule) rulesClean.Clone();
           
            ObservableCollection<HistoryRecord> hist = new ObservableCollection<HistoryRecord>();
            Patient p = new Patient(r, hist, id);           
            patients.Add(p);
            SetActivePatient(p);
        }
        public void SetActivePatient(PatientInfo id)
        {
            foreach (var item in patients)
                if (id == item.Info)
                {
                    SetActivePatient(item);           
                }

        }
        public void ResetPages()
        {
            for(int i=1;i<Children.Count;i++)
            {
                ((IReset)Children[i]).Reset();
            }

        }
        public void RemoveChildren()
        {
            if (Children.Count > 1)
            {
                int count = Children.Count;
                for(int i=count-1;i>=1;i--)
                    Children.RemoveAt(i);
            }

        }
        public void SetActivePatient(Patient id)
        {
            RemoveChildren();

            if (Children.Count < id.pages.Count + 1)
            {
                for (int i = 0; i < id.pages.Count; i++)
                {
                    var nav = new NavigationPage(id.pages[i]);
                    Children.Add(nav);
                    //Children.Add(id.pages[i]);
                    Children[i + 1].Title = id.pages[i].Title;
                }
                ((SchemaPage)((NavigationPage)Children[2]).CurrentPage).RefreshPosition();
            }
   


        }

    }
}
