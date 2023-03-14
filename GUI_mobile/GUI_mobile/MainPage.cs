using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using RuleModule_Odyn;
using System.IO;
using System.Threading.Tasks;

namespace GUI_mobile
{
    class MainPage : ContentPage
    {
        RuleModule rules;
        Button initSchema;
        public MainPage(RuleModule rules)
        {
            //var img = new Image() { Source = FileImageSource.FromUri(new Uri("https://www.planetware.com/wpimages/2020/02/france-in-pictures-beautiful-places-to-photograph-eiffel-tower.jpg")) };
            var img = new Image() { Source = FileImageSource.FromFile("resp2.jpg") };
            //this.BackgroundImageSource = "i-superstar-respirator-s1100.jpg";
            this.rules = rules;            
            this.Padding = new Thickness(20, 20, 20, 20);
            
            StackLayout panel = new StackLayout
            {
                Spacing = 15,
               
            };
            
            panel.Children.Add(img);
            panel.Children.Add(initSchema = new Button
            {
                Text = "START",
                FontSize=50,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            });
            initSchema.Clicked += InitSchema;
            var relativeLayout = new RelativeLayout();
            relativeLayout.Children.Add(img,
               Constraint.Constant(0),
               Constraint.Constant(0),
               Constraint.RelativeToParent((parent) => { return parent.Width; }),
               Constraint.RelativeToParent((parent) => { return parent.Height; }));

            relativeLayout.Children.Add(panel,
                Constraint.Constant(0),
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToParent((parent) => { return parent.Height; }));

            //new ScrollView { Content =
            Content = relativeLayout;
            //this.Content =panel;
            Device.BeginInvokeOnMainThread(() => { InData(); });
            Device.BeginInvokeOnMainThread(() => { SugestionData(); });
            Device.BeginInvokeOnMainThread(() => { AlarmNote(); });
        }
       
        public async void InData()
        {
            while (true)
            {
                if (StaticData.inputData != null && StaticData.inputData.Count > 0)
                {
                    InputDataObject k;
                    StaticData.inputData.TryDequeue(out k);

                    if (StaticData.inputData.Count > 0)
                        Console.WriteLine("UPS");

                    if (!PageInput.inputFlag)
                    {
                        PageInput p = new PageInput(k);
                        await App.Current.MainPage.Navigation.PushAsync(p);
                    }
                    
                }
                await Task.Delay(100);
            }
           
        }
        public async void AlarmNote()
        {
            while (true)
            {
                if (StaticData.alarmMessages != null && StaticData.alarmMessages.Count > 0)
                {
                    InputDataObject l;
                    lock (StaticData.alarmMessages)
                    {
                        StaticData.alarmMessages.TryDequeue(out l);
                    }
                    
                    if (l.node.expression.Count == 2)
                    {
                        await DisplayAlert(l.node.expression[0], l.node.expression[1], "OK");
                        
                    }
                    else
                        await DisplayAlert("Alarm", "Niepoprawnie zdefiniowany alarm", "OK");

                    l.hl.Set();

                }
                await Task.Delay(100);
            }

        }
        public async void SugestionData()
        {
            while (true)
            {
                if (StaticData.sugestions != null && StaticData.sugestions.Count > 0)
                {
                    InputDataObject l;
                    lock (StaticData.sugestions)
                    {
                        StaticData.sugestions.TryDequeue(out l);
                    }
                    if (l.node.sugestions[0].Item1.Contains("sleepTime"))
                    {
                        TimerPage p = new TimerPage(l);
                        await App.Current.MainPage.Navigation.PushAsync(p);
                    }
                    else
                    {
                        SugestionPage p = new SugestionPage(l);
                        await App.Current.MainPage.Navigation.PushAsync(p);
                    }
                   
                }
                await Task.Delay(100);
            }

        }

        public void InitSchema(object sender, EventArgs e)
        {
            rules.StartSchemaInThread((Schema)rules.rules["Grudziadz"].Clone());
            //TimerPage p = new TimerPage(7300);
            //App.Current.MainPage.Navigation.PushAsync(p);
            //Tuple<bool, List<string>> nodeRes =rules.rules["INIT"].RunNext();       
            //Tuple<bool, List<string>> nodeRes = rules.rules["INIT"].RunNext();
            /* while (nodeRes.Item1 == true)
             {                
                     nodeRes = rules.rules["INIT"].RunNext();             
             }*/

        }
    }
}
