using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RuleModule_Odyn;
using System.Threading;


namespace GUI_mobile
{
    


    public class PageInput : ContentPage
    {
        Button done;
        public static bool inputFlag=false;
      
        public Dictionary<string, double> varValues;
        EventWaitHandle hl;

        Dictionary<string, Entry> entries=new Dictionary<string, Entry>();
        RadioButton women = null;



        public PageInput(InputDataObject k)
        {
            PageInput.inputFlag = true;
            this.BackgroundImageSource = "";
            NavigationPage.SetHasBackButton(this, false);
            this.Padding = new Thickness(20, 20, 20, 20);
            hl = k.hl;
            var panel = new Grid();
            for(int i=0;i<k.node.input.Count;i++)
                panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60, GridUnitType.Absolute) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.33, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.33, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.33, GridUnitType.Star) });

        
            int size = (int)Application.Current.MainPage.Height / 25;
            int n = 0;
            
            foreach (var item in k.node.input)
            {
                if (item.Contains("sex"))
                {
                   
                    women = new RadioButton { Content = "Kobieta",GroupName="1",IsChecked=true};
                    
                    var  man= new RadioButton { Content ="Mężczyzna",GroupName="1" };

                    panel.Children.Add(new Label
                    {
                        Text = "Płeć",
                        FontSize = size
                    },0,n);
                   
                    panel.Children.Add(women,1,n);
                    panel.Children.Add(man,2,n);
                
                }
                else
                {
                    //var en=new MyEntryRenderer
                    var en = new Entry
                    {
                        Keyboard = Keyboard.Telephone,
                        FontSize=size,                        
                    };
                   
                    entries.Add(item, en);
                    string lab = item;
                    if (StaticData.translate.ContainsKey(item))
                        lab = StaticData.translate[item];
                        panel.Children.Add(new Label
                    {
                        Text = lab,
                        FontSize = size
                    },0,n);
                    panel.Children.Add(en,1,n);
                }
                n++;
            }
            
            //panel.Orientation = StackOrientation.Horizontal;
            
            panel.Children.Add(done = new Button
            {
                Text = "Zatwierdź",
                FontSize=size

            },2,n);
            done.HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
            done.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
           
            done.Clicked += GetData;
            ScrollView scrollView = new ScrollView { Content = panel};
            this.Content = scrollView;
        }
        void PageSetup()
        {

        }
        
        public void GetData(object sender, EventArgs e)
        {
            varValues = new Dictionary<string, double>();

            foreach (var item in entries)
            {
                if (item.Value.Text != null && item.Value.Text.Length > 0)
                {
                    try
                    {
                        varValues.Add(item.Key, Convert.ToDouble(item.Value.Text));
                    }
                    catch(Exception ex)
                    {
                        DisplayAlert("Alarm", "Niepoprawna wartosc dla pola "+item.Key, "OK");
                        return;
                    }
                }
                else
                    return;
            }
            if (women != null)
                if (women.IsChecked)
                    varValues.Add("sex", 0);
                else
                    varValues.Add("sex", 1);
            Node.SetVariables(varValues);
            PageInput.inputFlag = false ;
            hl.Set();
            this.Navigation.PopAsync();
            
        }
    }
}