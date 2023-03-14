using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using RuleModule_Odyn;

namespace GUI_mobile
{
    class TimerPage : ContentPage
    {               
        Button addData;
        Label timeLabel;
        int currentTime;
        int hours = 0;
        int minuts = 0;
        int seconds = 0;
        InputDataObject data = null;
        public TimerPage(InputDataObject data)//in sec
        {
            NavigationPage.SetHasBackButton(this, false);
            int size = (int)Application.Current.MainPage.Height / 25;
            this.data = data;

            currentTime = (int)data.node.sugestions[0].Item2;
            GetTime(currentTime);
            timeLabel = new Label
            {
                Text = TimerToString(),
                FontSize = size*4,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            if (data.node.notes==null || data.node.notes.Count ==0)
            {

                addData = new Button { Text = "Wprowadzanie wyników gazometrii", FontSize = size };
                Content = new StackLayout
                {

                    Children = {
                    timeLabel,
                    addData
                }


                };
                addData.Clicked += Next;
            }
            else
                Content = new StackLayout
                {

                    Children = {
                        timeLabel

                    }
                };
                
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                // do something every 60 seconds
                Device.BeginInvokeOnMainThread(() =>
                {
                    currentTime -= 1;
                    GetTime(currentTime);
                    timeLabel.Text = TimerToString();
                });
                if (currentTime <= 1)
                {
                    if (data.node.notes != null && data.node.notes.Count > 0)
                    {
                        data.hl.Set();
                        this.Navigation.PopAsync();
                    }
                    return false;
                }

                return true; // runs again, or false to stop)
            });
            

        }
        string TimerToString()
        {
            string timeString="";
            if (hours < 10)
                timeString += "0" + hours.ToString();
            else
                timeString += hours.ToString();
            timeString += ":";
            if(minuts<10)
                timeString += "0" + minuts.ToString();
            else
                timeString += minuts.ToString();
            timeString += ":";
            if (seconds < 10)
                timeString += "0" + seconds.ToString();
            else
                timeString += seconds.ToString();


            return timeString;
        }
        void GetTime(int timeSet)
        {
            hours = timeSet/(3600);
            int x = timeSet - hours * (3600);
            minuts = x/(60);

            seconds = x - minuts*60;
          
        }
        public void Next(object sender, EventArgs e)
        {
            data.hl.Set();
            this.Navigation.PopAsync();
        }
    }
}
