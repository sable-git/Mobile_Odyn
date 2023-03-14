using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RuleModule_Odyn;
using Xamarin.Forms;

namespace GUI_mobile
{
    public class MyButton:Button
    {
        public string explanation;
    }
    public class SugestionPage : ContentPage
    {
        InputDataObject sug;
        Button done;
        Button edit;
        public SugestionPage(InputDataObject sug)
        {
            NavigationPage.SetHasBackButton(this, false);
            int size = (int)Application.Current.MainPage.Height / 25;
            this.sug = sug;
            Title = "Sugestie";
            NavigationPage.SetHasBackButton(this, false);
            done = new Button { Text = "Zatwierdź",FontSize=size };

            var panel = new Grid();
            for (int i = 0; i < sug.node.sugestions.Count; i++)
                panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60, GridUnitType.Absolute) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.33, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.33, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.33, GridUnitType.Star) });


            /*            StackLayout panel = new StackLayout
                        {
                            Spacing = 15
                        };*/
            int n = 0;
            foreach (var item in sug.node.sugestions)
            {
                
                panel.Children.Add(new Label
                {
                    Text = item.Item1,
                    FontSize = size,
                    TextColor=Color.Red
                },0,n);
                string valStr="";
                 if(Math.Truncate(item.Item2) == item.Item2)
                {
                    valStr = ((int)Math.Round(item.Item2)).ToString();
                }
                 else
                {

                    valStr = item.Item2.ToString("N1");

                }

                panel.Children.Add(new Label
                {                   
                    Text =valStr,
                    FontSize = size,
                    TextColor = Color.Red
                }, 1, n);

                n++;
            }
            done.Clicked += Done;
            
            
            if (sug.node.remarks != null && sug.node.remarks.Count > 0)
            {
                MyButton aux = new MyButton { Text = "Wyjaśnienie", FontSize = size, explanation = sug.node.remarks[0] };
                aux.Clicked += Explain;
                aux.HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
                aux.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
                panel.Children.Add(aux, 2, n);
            }

            n++;
            panel.Children.Add(done, 2, n);
            done.HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
            done.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
            panel.Children.Add(edit = new Button { Text = "Edytuj", FontSize = size }, 0, n);
            edit.Clicked += Edit;
            edit.HorizontalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
            edit.VerticalOptions = new LayoutOptions { Alignment = LayoutAlignment.Center };
            ScrollView scrollView = new ScrollView { Content = panel };
            this.Content = scrollView;
        }
        public void Done(object sender, EventArgs e)
        {
            sug.hl.Set();
            this.Navigation.PopAsync();
        }
        public async void Explain(object sender, EventArgs e)
        {
            
            await DisplayAlert("Wyjaśnienie",( (MyButton)sender).explanation, "OK");
        }
        public void Edit(object sender, EventArgs e)
        {
            this.Navigation.PopAsync();
            InputDataObject k = new InputDataObject();
            Node x = new SugestionNode("aux");
            x.input = new List<string>();
            k.hl = sug.hl;
            foreach (var item in sug.node.sugestions)
                x.input.Add(item.Item1);
            k.node = x;
            PageInput p = new PageInput(k);
            this.Navigation.PushAsync(p);
            //k.hl.WaitOne();
            //sug.hl.Set();
            //this.Navigation.PopAsync();
        }
    }
}