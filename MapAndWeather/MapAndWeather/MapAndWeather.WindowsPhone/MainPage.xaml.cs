using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Newtonsoft.Json;
using System.Net.Http;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;

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
			GetCoordinatesOnCurrentlyPosition();
			this.NavigationCacheMode = NavigationCacheMode.Required;
			mySlider.Value = MapControl.ZoomLevel;
		}

		private async void GetCoordinatesOnCurrentlyPosition()
		{
			Geolocator myGeolocator = new Geolocator();
			Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
			Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
			geolocation.Text = myGeocoordinate.Latitude.ToString() + " " + myGeocoordinate.Longitude.ToString();
			GetWeatherOnCurrentlyPosition(myGeocoordinate.Latitude, myGeocoordinate.Longitude);
		}

		/// <summary>
		/// Wywoływane, gdy ta strona ma być wyświetlona w ramce.
		/// </summary>
		/// <param name="e">Dane zdarzenia, opisujące, jak została osiągnięta ta strona.
		/// Ten parametr jest zazwyczaj używany do konfigurowania strony.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// TODO: Tutaj przygotuj stronę do wyświetlenia.

			// TODO: Jeśli aplikacja zawiera wiele stron, upewnij się, że jest
			// obsługiwany przycisk Wstecz sprzętu, rejestrując
			// zdarzenie Windows.Phone.UI.Input.HardwareButtons.BackPressed.
			// Jeśli używasz obiektu NavigationHelper dostarczanego w niektórych szablonach,
			// to zdarzenie jest już obsługiwane.
		}

		private async void GetWeatherOnCurrentlyPosition(double latitude, double longitude)
		{
			var client = new HttpClient();
			var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude));
			var result = await response.Content.ReadAsStringAsync();

			var res = JsonConvert.DeserializeObject<RootObject>(result);
			txtBlock.Text = string.Join("\n", res.name, CalculateKelvinToCelcius(res.main.temp) + " *C");

			foreach (var item in res.weather)
			{
				txtBlock.Text += "\n" + item.description;
				myImage.Source = new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + item.icon.ToString() + ".png"));
			}
		}

		private string CalculateKelvinToCelcius(double Kelvin)
		{
			double x = Kelvin - 273.15;
			return x.ToString("##.##");
		}

		private void MapControl_OnMapTapped(MapControl sender, MapInputEventArgs args)
		{
			var latitude = args.Location.Position.Latitude;
			var longitude = args.Location.Position.Longitude;
			GetWeatherFromCoordinates(latitude, longitude);
		}

		private async void GetWeatherFromCoordinates(double latitude, double longitude)
		{
			var client = new HttpClient();
			var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?lat="+ latitude + "&lon=" + longitude));
			var result = await response.Content.ReadAsStringAsync();

			var res = JsonConvert.DeserializeObject<RootObject>(result);
			ShowWeather.Text = string.Join("\n", res.name, CalculateKelvinToCelcius(res.main.temp) + " *C");
			
			foreach (var item in res.weather)
			{
				ShowWeather.Text += "\n" + item.description;
				myImageToSelectedWeather.Source = new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + item.icon.ToString() + ".png"));
			}

			ShowPosition.Text = latitude + "\n" + longitude;
		}

		private void MySlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (MapControl != null)
			{
				MapControl.ZoomLevel = e.NewValue;
			}
		}
	}
}
