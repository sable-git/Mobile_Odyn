using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GUI_mobile4
{
    class Test : ContentPage
    {
		public Test()
		{
			Padding = new Thickness(0, 20, 0, 0);

			var listView = new ListView();

			listView.ItemsSource = new PatientInfo[] {
				new PatientInfo {Id="a"},
				new PatientInfo {Id="b"},
			};

			listView.ItemTemplate = new DataTemplate(typeof(TextCell)); // has context actions defined
			listView.ItemTemplate.SetBinding(TextCell.TextProperty, "Id");

			listView.Header = new StackLayout
			{
				Padding = new Thickness(5, 10, 5, 0),
				BackgroundColor = Color.FromHex("#cccccc"),
				Children ={
					new Label {Text="Header"},
					new Label {Text="Subhead", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) }
				}
			};

			listView.Footer = new Label { Text = "Footer" };
			/*Content = new StackLayout
			{
				Children = {
					listView
				},
			};*/
			Content = listView;
		}

	}
}
