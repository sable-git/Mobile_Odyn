using RuleModule_Odyn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace GUI_mobile4
{
    public class PatientInfo
    {
        public string Id { get; set; }
        public int FontSize { get; set; }
    }
    public class Patient
    {
        public List<Page> pages=new List<Page>();
        public PatientInfo Info { get; set; }
        public RuleModule r;

        public Patient(RuleModule r, ObservableCollection<HistoryRecord> h,PatientInfo info)
        {
            this.r = r;
            this.Info = info;
            Page page1 = new MainPage(r, h) { Title = "Sugestie" };
            Page page2 = new SchemaPage(r) { Title = "Aktywny schemat" };
            Page page3 = new HistoryPage(h) {Title = "Historia"};            
            pages.Add(page1);
            pages.Add(page2);
            pages.Add(page3);
        }

    }

}
