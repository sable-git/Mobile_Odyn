using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GUI_mobile4
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Popup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public Popup(List<string> text)
        {
            InitializeComponent();
            foreach(var item in text)
            {
                
                Label l = new Label()
                {
                    Text = item,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Color.MistyRose,
                    FontSize=35
                };
                stack.Children.Add(l);
            }
            
            labelTitle.Text = "ALARM";
            labelTitle.FontSize = 35;
                    
           // this.BackgroundImageSource = imS;
           this.Opacity = 0.3;
            this.BackgroundColor = new Color(0, 0, 0, 0.7);
            //this.BackgroundInputTransparent = true;
            WarningSign();
        }
        async void WarningSign()
        {
            bool test = true;
            while (true)
            {
                if (test)
                {
                    myImage.Source = "warningSign.png";
                    test = false;
                }
                else
                {
                    myImage.Source = "";
                    test = true;
                }

                await Task.Delay(1000);
            }
        }

        public void OnAnimationStarted(bool isPopAnimation)
        {
            // optional code here   
        }


        public void OnAnimationFinished(bool isPopAnimation)
        {
            // optional code here   
        }

        protected override bool OnBackButtonPressed()
        {
            Console.WriteLine("CLICK HEST zzzzz");
            // Return true if you don't want to close this popup page when a back button is pressed
            return true;
        }


        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            Console.WriteLine("CLICK HEST dddd");
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false;
        }

        private async void CloseBtn_Clicked(object sender, EventArgs e)
        {            
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}