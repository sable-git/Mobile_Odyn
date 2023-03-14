using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RuleModule_Odyn;
using System.Threading;
using System.Collections.ObjectModel;
using System.Reflection;

namespace GUI_mobile4
{
    public class ExtendedEntry : Entry
    {
        public string entryName="";
        public bool isValid = true;
    }
    public class NumericValidationBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            double result;
            bool isValid = false;
            double.TryParse(args.NewTextValue, out result);
            ExtendedEntry e = (ExtendedEntry)sender;
            if (e.entryName.Length > 0)
            {
                if (StaticElements.dataFormat.ContainsKey(e.entryName))
                {
                    string[] aux = StaticElements.dataFormat[e.entryName]["range"].Split('-');
                    double.TryParse(aux[0], out double f);
                    double.TryParse(aux[1], out double t);
                    if (result >= f && result <= t)
                        isValid = true;
                }
                else
                    isValid = true;

            }
            else
                isValid = true;

            e.isValid = isValid;
            ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }
    }
    public class PageInput : ContentPage
    {
        readonly Button done;
        public bool runPage = true;
      
        public Dictionary<string, double> varValues;
        readonly EventWaitHandle hl;

        readonly Dictionary<string, ExtendedEntry> entries=new Dictionary<string, ExtendedEntry>();
        readonly Dictionary<string, ExtendedLabel> dicLabel;
        readonly RadioButton women = null;
        readonly RuleModule rules;
        readonly MainPage p;
        readonly HashSet<string> dependencies = new HashSet<string>() { "RR", "TiTe", "Ti" ,"Vt","Vt1kg"};
        Dictionary<string, double> val;
        public PageInput(MainPage p, Dictionary<string, double> val,string title="",bool oldValue=false)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            this.val = val;
            this.p = p;
            this.rules = p.rules;
            runPage = false;
            varValues = new Dictionary<string, double>();
            this.Title = title;
            this.dicLabel = p.entries;
            this.BackgroundImageSource = "";
            NavigationPage.SetHasBackButton(this, false);
            this.Padding = new Thickness(20, 20, 20, 20);
            hl = p.k.hl;
            var panel = new Grid();
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            if(oldValue)
                panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.03, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            for(int i=0;i<val.Count+1;i++)
                panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            panel.HorizontalOptions = LayoutOptions.Center;
            panel.VerticalOptions = LayoutOptions.Center;
            int refPosition = 0;
            if (oldValue)
            {
                refPosition=1;
                    panel.Children.Add(new Label
                {
                    Text = "Aktualne",
                    FontSize = StaticElements.fontSize
                }, 1, 0);
                panel.Children.Add(new Label
                {
                    Text = "Sugerowane",
                    FontSize = StaticElements.fontSize
                }, 2, 0);

            }
            int n = 1;
            foreach (var item in val)
            {
                if (item.Key.Contains("sex"))
                {
                   
                    women = new RadioButton { Content = "Kobieta",GroupName="1",IsChecked=true, VerticalOptions = LayoutOptions.Center };
                    
                    var  man= new RadioButton { Content ="Mężczyzna",GroupName="1",VerticalOptions=LayoutOptions.Center };

                    panel.Children.Add(new Label
                    {
                        Text = "Płeć",
                        FontSize = StaticElements.fontSize
                    },0,n);
                   
                    panel.Children.Add(women,1+refPosition,n);
                    panel.Children.Add(man,3+refPosition,n);
                
                }
                else
                {
                    string x = "";
                    if (item.Value >= 0)
                        if (StaticElements.dataFormat.ContainsKey(item.Key))
                            x = String.Format(StaticElements.dataFormat[item.Key]["format"], item.Value);
                        else
                            x =item.Value.ToString("N1");
                    //var en=new MyEntryRenderer
                    string lab = item.Key;
                    if (StaticData.translate.ContainsKey(item.Key))
                        lab = StaticData.translate[item.Key];

                    Label ls = new Label
                    {
                        Text = lab,
                        FontSize = StaticElements.fontSize,
                        VerticalTextAlignment=TextAlignment.Center
                    };

                    if(refPosition>0 && p.remValues.ContainsKey(item.Key))
                    {
                        string vv;
                        if (!p.remValues.ContainsKey(item.Key))
                            vv = "--";
                        else
                            vv = p.remValues[item.Key];
                      
                        if(item.Key.Contains("TiTe"))
                        {
                            double q = Convert.ToDouble(vv);
                            q = 1 / q;
                            vv = "1:" + String.Format(StaticElements.dataFormat["TiTe"]["format"],q); 
                        }
                        if(vv.Contains("[?]"))
                        {
                            string []s = vv.Split('[');
                            vv = s[0];
                        }
                        Label oValue = new Label
                        {
                            Text =vv ,
                            FontSize = StaticElements.fontSize,
                            VerticalTextAlignment = TextAlignment.Center
                        };
                        panel.Children.Add(oValue,1, n);
                    }

                    ExtendedEntry en;
                    en = new ExtendedEntry
                    {
                        Text = x,
                        entryName = item.Key,
                        Keyboard = Keyboard.Telephone,
                        FontSize=StaticElements.fontSize,                        
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    en.Behaviors.Add(new NumericValidationBehavior());
                    entries.Add(item.Key, en);
                    
                   
                    panel.Children.Add(ls,0,n);
                    panel.Children.Add(en,1+refPosition,n);

                    if(StaticElements.dataFormat.ContainsKey(item.Key))
                    {
                        if (StaticElements.dataFormat[item.Key]["unit"].Length > 0)
                        {
                            Label unit = new Label
                            {
                                Text = StaticElements.dataFormat[item.Key]["unit"],
                                FontSize = StaticElements.fontSize,
                                TextColor=Color.Black,
                                VerticalTextAlignment = TextAlignment.Center
                            };
                            panel.Children.Add(unit, panel.ColumnDefinitions.Count-1, n);
                        }
                    }
                    double v = 1.0;
                    if (item.Key == "TiTe")
                    {
                        ls.Text = "Ti:Te";
                        string vs = "";
                        en.Text = v.ToString();
                        if (item.Value > 0)
                        {                            
                            double val1 = 1 / item.Value;
                            vs = String.Format("{0:0.#}", val1);
                        }


                        panel.Children.Add(new Label
                        {
                            Text = ":",
                            FontSize = StaticElements.fontSize,
                            VerticalTextAlignment = TextAlignment.Center
                        }, 2+refPosition, n);

                        en = new ExtendedEntry
                        {
                            Text = vs,
                            Keyboard = Keyboard.Telephone,
                            FontSize = StaticElements.fontSize,
                            VerticalTextAlignment = TextAlignment.Center
                        };
                        entries.Add(item.Key + "2", en);
                        panel.Children.Add(en, 3+refPosition, n);
                        
                    }

                }
                n++;
            }
            
            //panel.Orientation = StackOrientation.Horizontal;
            
            panel.Children.Add(done = new Button
            {
                Text = "Zatwierdź",
                FontSize=StaticElements.fontSize

            }, panel.ColumnDefinitions.Count-1, n);
            done.HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
            done.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };

            foreach(var item in entries)
            {
                if(dependencies.Contains(item.Key))
                {
                    item.Value.Unfocused += RRTiChaged;
                }
            }
           
            done.Clicked += GetData;
            ScrollView scrollView = new ScrollView { Content = panel};
            this.Content = scrollView;
        }
        
        public void RRTiChaged(object sender, EventArgs e)
        {
            ExtendedEntry b = (ExtendedEntry)sender;

            double x = Convert.ToDouble(b.Text);
           switch (b.entryName)
            {
                case "RR":
                    if(entries.ContainsKey("Ti"))
                    {
                        ExtendedEntry ti = entries["Ti"];
                        Dictionary<string, double> zz = p.rules.GetVariables();
                        if (zz.ContainsKey("TiTe"))
                        {
                            double dTiTe = zz["TiTe"];
                            double Ti = dTiTe * 60 / (x * (dTiTe+1));

                            ti.Text = String.Format(StaticElements.dataFormat["Ti"]["format"], Ti);
                        }
                    }
                    break;
                case "Ti":
                    if (entries.ContainsKey("RR"))
                    {
                        ExtendedEntry rr = entries["RR"];
                        Dictionary<string, double> zz = p.rules.GetVariables();
                        if (zz.ContainsKey("TiTe"))
                        {
                            double dTiTe = zz["TiTe"];
                            double RR = dTiTe * 60 / (x * (dTiTe + 1));

                            rr.Text = String.Format(StaticElements.dataFormat["RR"]["format"], RR);
                        }
                    }
                    break;
                case "Vt":
                    if (entries.ContainsKey("Vt1kg"))
                    {
                        ExtendedEntry rr = entries["Vt1kg"];
                        Dictionary<string, double> zz = p.rules.GetVariables();
                        if (zz.ContainsKey("Vt1kg"))
                        {
                            double vtkg= Convert.ToDouble(entries["Vt"].Text);
                            double RR = vtkg/ zz["PBW"];

                            rr.Text = String.Format(StaticElements.dataFormat["Vt1kg"]["format"], RR);
                        }
                    }
                    break;
                case "Vt1kg":
                    if (entries.ContainsKey("Vt1kg"))
                    {
                        ExtendedEntry rr = entries["Vt"];
                        Dictionary<string, double> zz = p.rules.GetVariables();
                        if (zz.ContainsKey("Vt"))
                        {
                            double vt = Convert.ToDouble(entries["Vt1kg"].Text);
                            double RR = vt* zz["PBW"];

                            rr.Text = String.Format(StaticElements.dataFormat["Vt"]["format"], RR);
                        }
                    }
                    break;



            }

        }
            

        public void GetData(object sender, EventArgs e)
        {
            varValues.Clear();      

            foreach (var item in entries)
            {
                if(item.Value.entryName.Length>0 && !item.Value.isValid)
                {
                    DisplayAlert("Błąd", "Niepoprawna warość "+item.Value.entryName, "OK");
                    return;
                }
                if (item.Key == "TiTe2")
                    continue;
                if (item.Key=="TiTe")
                {
                    try
                    {
                        double c = Convert.ToDouble(item.Value.Text) / Convert.ToDouble(entries[item.Key + "2"].Text);
                        varValues.Add(item.Key, c);
                    }
                    catch (Exception)
                    {
                        DisplayAlert("Alarm", "Niepoprawna wartosc dla pola " + item.Key, "OK");
                        return;
                    }
                }
                else
                if (item.Value.Text != null && item.Value.Text.Length > 0)
                {
                    try
                    {
                        varValues.Add(item.Key, Convert.ToDouble(item.Value.Text));
                    }
                    catch(Exception)
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
            foreach(var item in varValues)
            {
                if (p.remValues.ContainsKey(item.Key))
                    p.remValues.Remove(item.Key);
                if(StaticElements.dataFormat.ContainsKey(item.Key))
                    p.remValues.Add(item.Key, String.Format(StaticElements.dataFormat[item.Key]["format"], item.Value));
            }
            rules.SetVariables(varValues);
            foreach(var item in varValues)
            {
                if (dicLabel.ContainsKey(item.Key))
                {
                    if(StaticElements.dataFormat.ContainsKey(item.Key))
                        dicLabel[item.Key].Text = String.Format(StaticElements.dataFormat[item.Key]["format"], item.Value);
                    dicLabel[item.Key].ClearClick();
                }
                if (p.sugestions.Contains(item.Key))
                {
                    if (p.sugestionValues.ContainsKey(item.Key))
                        p.sugestionValues.Remove(item.Key);
                    p.sugestionValues.Add(item.Key, item.Value);
                }
            }            
            p.ResetBlinking(false);
            hl.Set();            
            this.Navigation.PopAsync();
            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            p.runInputPage = false;
        }

    }

}