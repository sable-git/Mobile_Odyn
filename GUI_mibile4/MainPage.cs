using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using RuleModule_Odyn;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Reflection;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;

namespace GUI_mobile4
{
    public enum LabType
    {
        INPUT,
        SUGEST,
        CALC
    }
    public class ExtendedLabel : Label
    {
        private event EventHandler Click;
        public LabType lType;
        public bool blinking = false;

        public string Name
        {
            get; set;
        }

        public void DoClick()
        {
            Click?.Invoke(this, null);
        }
        public void ClearClick()
        {
            Click = null;
            GestureRecognizers.Clear();
        }
        public event EventHandler Clicked
        {
            add
            {
                lock (this)
                {
                    if (Click == null)
                    {
                        Click += value;
                        var g = new TapGestureRecognizer();
                        g.Tapped += (s, e) => Click?.Invoke(s, e);
                        GestureRecognizers.Add(g);
                    }
                }
            }
            remove
            {
                lock (this)
                {
                    Click=null;
                    GestureRecognizers.Clear();
                }
            }
        }
    }
    public class MainPage : ContentPage, IReset
    {
        public InputDataObject k;
        public bool runInputPage = false;
        EventWaitHandle current = null;
        public RuleModule rules;
        readonly Label timeLabel = null;
        private readonly object varLock = new object();
        public Dictionary<string, ExtendedLabel> entries = new Dictionary<string, ExtendedLabel>();
        public readonly ObservableCollection<HistoryRecord> history;
        readonly Dictionary<string, double> remSugestions = new Dictionary<string, double>();
        public Dictionary<string, string> remValues = new Dictionary<string, string>();
        readonly Dictionary<string, LabType> settings = new Dictionary<string, LabType>();
        int currentTime;
        int hours = 0;
        int minutes = 0;
        int seconds = 0;
        readonly Grid panel = new Grid();
        public readonly HashSet<string> sugestions = new HashSet<string>()
            {
            "RR","TiTe","Ti","PEEP","FiO2","Vt1kg","Vt"
            };
        public Dictionary<string, double> sugestionValues = new Dictionary<string, double>();
        readonly Dictionary<string, Tuple<LabType, int, int, int, int>> vData = new Dictionary<string, Tuple<LabType, int, int, int, int>>()
            {
                { "sex",new Tuple<LabType, int, int,int,int>
                (LabType.INPUT ,1,0,1,1) },
                { "height",new Tuple<LabType, int, int,int,int>
                (LabType.INPUT ,2,0,2,1) },
                { "TiTe",new Tuple<LabType, int, int, int, int>
                (LabType.SUGEST,0,3,1,3)},
                { "RR",new Tuple<LabType, int, int, int, int>
                (LabType.SUGEST,0,4,1,4)},
                { "Ti",new Tuple<LabType, int, int, int, int>
                (LabType.SUGEST,0,5,1,5)},
                { "PEEP",new Tuple<LabType, int, int, int, int>
                (LabType.SUGEST,0,7,1,7)},
                { "FiO2",new Tuple<LabType, int, int, int, int>
                (LabType.SUGEST,0,8,1,8)},
                { "MV",new Tuple<LabType, int, int, int, int>
                (LabType.CALC,3,3,4,3)},
                { "Vt1kg",new Tuple<LabType, int, int, int, int>
                (LabType.SUGEST,3,4,4,4)},
                { "Vt",new Tuple<LabType, int, int, int, int>
                (LabType.SUGEST,3,5,4,5)},
                { "Ppeak",new Tuple<LabType, int, int, int, int>
                (LabType.INPUT,3,7,4,7)},
                { "SpO2",new Tuple<LabType, int, int, int, int>
                (LabType.INPUT,3,8,4,8)},
                { "PaO2",new Tuple<LabType, int, int, int, int>
                (LabType.INPUT,1,10,1,11)},
                { "pCO2",new Tuple<LabType, int, int, int, int>
                (LabType.INPUT,2,10,2,11)},
                { "pH",new Tuple<LabType, int, int, int, int>
                (LabType.INPUT,3,10,3,11)}

            };
        private IPopupNavigation PopupI { get; set; }
        public MainPage(RuleModule rules, ObservableCollection<HistoryRecord> h)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            history = h;

            PopupI = PopupNavigation.Instance;

            List<Tuple<string,int,int>> labelsBlue = new List<Tuple<string, int, int>>
            { new Tuple<string,int,int>("SUGESTIE",0,2),
              new Tuple<string,int,int>("ODCZYTY",3,6),
              new Tuple<string,int,int>("GAZOMETRIA",0,10) };
            
            this.rules = rules;
            double size= StaticElements.fontSize;

            
            for (int i = 0; i < 14; i++)
                //panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0/14, GridUnitType.Star) });
                panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });

            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            

            timeLabel = new Label { Text = "", FontSize = 3 * size, HorizontalTextAlignment = TextAlignment.Center };
            panel.Children.Add(timeLabel, 1, 3);
            Grid.SetColumnSpan(timeLabel, 3);
            Grid.SetRowSpan(timeLabel, 3);

            foreach (var item in vData)
            {
                    string aux = item.Key;
                if (StaticData.translate.ContainsKey(aux))
                    aux = StaticData.translate[aux];

                panel.Children.Add(new Label
                    {
                        Text = aux,
                        FontSize = size,
                        HorizontalTextAlignment = TextAlignment.Center                                              
                    }, item.Value.Item2, item.Value.Item3); 
                    var lab = new ExtendedLabel
                    {
                        lType = item.Value.Item1,
                        Text = "--",
                        FontSize = size,
                        TextColor=Color.Black,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center

                    };
                    panel.Children.Add(lab, item.Value.Item4, item.Value.Item5);
                    this.entries.Add(item.Key, lab);

                }
            foreach (var item in labelsBlue)
            {
                panel.Children.Add(new Label
                {
                    Text = item.Item1,
                    FontSize = size,
                    TextColor = Color.Blue,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center
                }, item.Item2, item.Item3);
            }            

            this.Content =panel;

            Device.BeginInvokeOnMainThread(() => {InData(); });            
            Device.BeginInvokeOnMainThread(() => { SugestionData(); });
            Device.BeginInvokeOnMainThread(() => { AlarmNote(); });
            rules.StartActiveSchemaInThread();
        }
        public void Reset()
        {
            foreach (var item in entries)
            {
                item.Value.Text = "--";
            }            
            ResetClock();
            ResetBlinking(true);
            rules.Reset();
            history.Clear();
        }
        void GetTime(int timeSet)
        {

            if (timeSet >= 0)
            {
                hours = timeSet / (3600);
                int x = timeSet - hours * (3600);
                minutes = x / (60);

                seconds = x - minutes * 60;
            }

        }
        string TimerToString()
        {
            string timeString = "";
            if (hours < 10)
                timeString += "0" + hours.ToString();
            else
                timeString += hours.ToString();
            timeString += ":";
            if (minutes < 10)
                timeString += "0" + minutes.ToString();
            else
                timeString += minutes.ToString();
            timeString += ":";
            if (seconds < 10)
                timeString += "0" + seconds.ToString();
            else
                timeString += seconds.ToString();


            return timeString;
        }
        public async void ClickLabel(object sender, EventArgs e)
        {
            PageInput p;
            bool peepTest = false;
            if (!runInputPage)
            {
                runInputPage = true;
                ResetClock();
                Dictionary<string, double> z = new Dictionary< string, double>();
                string title="";
                if (k.node.input != null)
                {

                    foreach (var item in k.node.input)
                    {
                        if (vData.ContainsKey(item))
                        {                            
                            z.Add(item, -1);                            
                            if (settings.ContainsKey(item))
                                settings.Remove(item);

                            settings.Add(item, LabType.INPUT);
                        }
                    }
                    if(k.node.input.Count>0)
                        title = "Wprowadzanie danych";
                }
                if (k.node.sugestions != null)
                {
                    
                    foreach (var item in k.node.sugestions)
                    {
                        if (vData.ContainsKey(item.Item1))
                        {
                            if (item.Item1.Contains("PEEP") || k.node.runContinue == false)
                                peepTest = true;
                            //z.Add(new Tuple<string, double>(item.Item1, item.Item2));
                            if (sugestionValues.ContainsKey(item.Item1))
                                sugestionValues.Remove(item.Item1);
                            sugestionValues.Add(item.Item1, item.Item2);
                            if (settings.ContainsKey(item.Item1))
                                settings.Remove(item.Item1);
                            settings.Add(item.Item1, LabType.SUGEST);
                        }
                    }
                    
                    if (title.Length == 0)
                        title = "Sugestia";
                }

                if (z.Count > 0 || z.Count == 0 && peepTest)
                {
                    if (z.Count > 0)
                        p = new PageInput(this, z, title);
                    else
                    {
                        foreach (var item in rules.GetVariables())
                            if (sugestions.Contains(item.Key) && !sugestionValues.ContainsKey(item.Key))
                                sugestionValues.Add(item.Key, item.Value);
                        p = new PageInput(this, sugestionValues, title,true);
                    }
                    //await App.Current.MainPage.Navigation.PushAsync(p);
                    await this.Navigation.PushAsync(p);
                }
                else                
                    this.k.hl.Set();
            }

        }
        void ResetClock()
        {
            lock (varLock)
            {
                minutes = 0;
                hours = 0;
                seconds = 0;
                currentTime = 0;
            }
        }
        async void InData()
        {
            while (true)
            {
                if (rules.inputData != null && rules.inputData.Count > 0)
                {                    
                    DisplayData();
                    rules.inputData.TryDequeue(out k);     
                    foreach(var item in k.node.input)
                    {
                        if(entries.ContainsKey(item))
                        {
                            entries[item].TextColor = Color.Black;
                            entries[item].Text += " [?]";
                            entries[item].Clicked += ClickLabel;
                            entries[item].blinking = true;
                            SoftBlink(entries[item],Color.FromRgb(30, 30, 30), Color.Red, 1000, false);
                        }    
                    }
                    // PageInput p = new PageInput(k,entries);
                    //await App.Current.MainPage.Navigation.PushAsync(p);
                    
                }
                await Task.Delay(100);
            }
           
        }
        async void AlarmNote()
        {
            while (true)
            {
                if (rules.alarmMessages != null && rules.alarmMessages.Count > 0)
                {
                    InputDataObject l;
                    lock (rules.alarmMessages)
                    {
                        rules.alarmMessages.TryDequeue(out l);
                    }

                    if (l.node.expression.Count == 2)
                    {
                        //                        await DisplayAlert(l.node.expression[0], l.node.expression[1], "OK");
                        var pop = new Popup(l.node.expression);
                        await PopupI.PushAsync(pop);
                    }
                    else
                    {
                        List<string> xx = new List<string>() { "Niepoprawnie zdefiniowany alarm" };
                        var pop = new Popup(xx);
                        await PopupI.PushAsync(pop);
                    }
                     //   await DisplayAlert("Alarm", "Niepoprawnie zdefiniowany alarm", "OK");
                    Reset();
                    l.hl.Set();
                    
                }
                await Task.Delay(100);
            }

        }
        void DisplayData()
        {
            foreach (var item in rules.GetVariables())
            {
                if (entries.ContainsKey(item.Key))
                {
                    if (item.Key.Contains("sex"))
                    {
                        if (item.Value == 0)
                            entries[item.Key].Text = "Kobieta";
                        else
                            entries[item.Key].Text = "Mężczyzna";
                    }
                    else
                    {                       
                        if (StaticElements.dataFormat.ContainsKey(item.Key))
                            entries[item.Key].Text = String.Format(StaticElements.dataFormat[item.Key]["format"], item.Value);
                        else
                            entries[item.Key].Text = String.Format("{0:0.#}", item.Value);
                    }
                }
            }
        }
        public void ResetBlinking(bool flagClick)
        {
            foreach (var item in entries)
            {
                item.Value.blinking = false;
                if(!flagClick)
                    item.Value.Clicked -=ClickLabel;
            }
        }
        async void SoftBlink(ExtendedLabel ctrl, Color c1, Color c2, short CycleTime_ms, bool BkClr)
        {
            ctrl.blinking = true;
            var sw = new Stopwatch(); sw.Start();
            short halfCycle = (short)Math.Round(CycleTime_ms * 0.5);
            while (ctrl.blinking)
            {
                await Task.Delay(1);
                var n = sw.ElapsedMilliseconds % CycleTime_ms;
                var per = (double)Math.Abs(n - halfCycle) / halfCycle;
                var red = (short)Math.Round((c2.R - c1.R) * per) + c1.R;
                var grn = (short)Math.Round((c2.G - c1.G) * per) + c1.G;
                var blw = (short)Math.Round((c2.B - c1.B) * per) + c1.B;
                var clr = Color.FromRgb(red, grn, blw);
                if (BkClr) ctrl.TextColor = clr; else ctrl.TextColor = clr;
            }
            ctrl.TextColor = Color.Black;
        }
        async void SugestionData()
        {
            while (true)
            {
                if (rules.sugestions != null && rules.sugestions.Count > 0)
                {
                    InputDataObject l;
                    lock (rules.sugestions)
                    {
                        rules.sugestions.TryDequeue(out l);
                        k = l;
                       
                        current = l.hl;
                    }
                    if (k.node.sugestions != null && k.node.sugestions.Count > 0)
                        foreach (var item in k.node.sugestions)
                            if(!remSugestions.ContainsKey(item.Item1))
                                remSugestions.Add(item.Item1, item.Item2);
                            else
                            {
                                remSugestions.Remove(item.Item1);
                                remSugestions.Add(item.Item1, item.Item2);
                            }
                    DisplayData();
                    if (l.node.sugestions[0].Item1.Contains("sleepTime"))
                    {
                        current.Set();
                        currentTime = (int)l.node.sugestions[0].Item2*60;
                        GetTime(currentTime);
                        DateTime startTime = DateTime.Now;
                        timeLabel.Text = TimerToString();
                        AddHistory();
                        Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                        {
                            DateTime currentT = DateTime.Now;
                            TimeSpan span=currentT.Subtract(startTime);
                            timeLabel.IsEnabled = true;
                            
                            // do something every 60 seconds
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                lock (varLock)
                                {

                                    //GetTime(currentTime);                                    
                                    GetTime((int)(currentTime-span.TotalSeconds));
                                    timeLabel.Text = TimerToString();
                                }
                            });
                            if (currentTime-span.TotalSeconds <= 1)
                            {
                                timeLabel.Text = "        ";
                                timeLabel.IsEnabled = false;                                                                                 
                                return false;
                            }

                            return true; // runs again, or false to stop)
                        });

                    }   
                    else
                    {
                        Dictionary<string, ExtendedLabel> zz = new Dictionary<string, ExtendedLabel>();
                        foreach (var en in entries)
                            if (en.Value.blinking)
                                zz.Add(en.Key,en.Value);
                        ResetBlinking(false);                       
                        foreach (var item in l.node.sugestions)
                        {
                            if(entries.ContainsKey(item.Item1))
                            {
                                if(StaticElements.dataFormat.ContainsKey(item.Item1))
                                    entries[item.Item1].Text = String.Format(StaticElements.dataFormat[item.Item1]["format"], item.Item2) + "[?]";
                                else
                                    entries[item.Item1].Text = String.Format("{0:0.#}", item.Item2)+"[?]";
                                entries[item.Item1].TextColor = Color.Black;
                                entries[item.Item1].Clicked += ClickLabel;
                                entries[item.Item1].blinking = true;
                                SoftBlink(entries[item.Item1], Color.FromRgb(30, 30, 30), Color.Red, 1000, true);
                            }
                            await Task.Delay(2);
                            foreach (var en in zz)
                            {
                                en.Value.Clicked += ClickLabel;
                                en.Value.blinking = true;
                                if(!en.Value.Text.Contains("?"))
                                    en.Value.Text += " [?]";
                                SoftBlink(en.Value, Color.FromRgb(30, 30, 30), Color.Red, 1000, true);
                            }

                        }
                        if (l.node.runContinue)
                        {

                            foreach (var item in l.node.sugestions)
                            {
                                    if (sugestionValues.ContainsKey(item.Item1))
                                        sugestionValues.Remove(item.Item1);
                                    sugestionValues.Add(item.Item1, item.Item2);
                                if(!entries[item.Item1].Text.Contains("?"))
                                    entries[item.Item1].Text += "[?]";
                            }

                                l.hl.Set();
                        }
                    }

                   
                }
                await Task.Delay(100);
            }

        }
        void AddHistory()
        {
            HistoryRecord h = new HistoryRecord();
            foreach (PropertyInfo prop in typeof(HistoryRecord).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                    h.GetType().GetProperty(prop.Name).SetValue(h, "--", null);
                //var n = h.GetType().GetField("h").GetValue(this);
                if (rules.GetVariables().ContainsKey(prop.Name))
                {

                    string vv;
                    string aux;
                    if (remSugestions.ContainsKey(prop.Name))
                    {
                        aux = String.Format("{0:0.#}", remSugestions[prop.Name]);
                        vv = String.Format("{0:0.#}", rules.GetVariables()[prop.Name]);
                        if(!(aux==vv))
                            vv=vv+"["+aux+"]";                       
                    }
                    else
                    {
                        vv = String.Format("{0:0.#}", rules.GetVariables()[prop.Name]);
                    }
                    if (StaticElements.dataFormat.ContainsKey(prop.Name))
                    {
                        if (remSugestions.ContainsKey(prop.Name))
                        {
                            aux = String.Format(StaticElements.dataFormat[prop.Name]["format"], remSugestions[prop.Name]);
                            vv = String.Format(StaticElements.dataFormat[prop.Name]["format"], rules.GetVariables()[prop.Name]);
                            if(!(aux==vv))
                                vv = vv + "[" + aux+"]";
                        }
                        else
                            vv = String.Format(StaticElements.dataFormat[prop.Name]["format"], rules.GetVariables()[prop.Name]);
                    }

                        if (settings.ContainsKey(prop.Name))
                    {
                        if(settings[prop.Name]==LabType.SUGEST)
                            h.GetType().GetProperty(prop.Name).SetValue(h, " " + vv, null);
                        else
                            h.GetType().GetProperty(prop.Name).SetValue(h,vv+" ", null);
                    }
                    else                        
                        h.GetType().GetProperty(prop.Name).SetValue(h, vv, null);
                }
            }
            h.Time = DateTime.Now.ToString();
            h.Id = history.Count + 1;
            history.Add(h);
            settings.Clear();
            remSugestions.Clear();

        }
    }
}
