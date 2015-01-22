using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Szablon elementu Pusta strona jest udokumentowany pod adresem http://go.microsoft.com/fwlink/?LinkId=234238

namespace MapAndWeather
{
	/// <summary>
	/// Pusta strona, która może być używana samodzielnie, lub do której można nawigować wewnątrz ramki.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		private async void btn_Click(object sender, RoutedEventArgs e)
		{
			var client = new HttpClient();
			var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=London,uk"));
			var result = await response.Content.ReadAsStringAsync();
			
			var res = JsonConvert.DeserializeObject<RootObject>(result);
			//txtBlock.Text = string.Join(" ", res.name, res.wind.speed);

			foreach(var item in res.weather)
			{
				//txtBlock.Text += item.description;
			}
		}


	}
}
