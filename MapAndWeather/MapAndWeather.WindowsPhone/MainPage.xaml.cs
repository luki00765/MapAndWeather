﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
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
		private List<WeatherFor3Days.List> weatherFor3DaysList = new List<WeatherFor3Days.List>();
		public MainPage()
		{
			this.InitializeComponent();
			Pivot.Background = new SolidColorBrush(Colors.SlateGray);
			ProgressRingOnCurrentlyPosition.Background = new SolidColorBrush(Colors.SlateGray);
			TglButton.Visibility = Visibility.Collapsed;
			ProgressRingOnCurrentlyPosition.IsActive = true;
			GetCoordinatesOnCurrentlyPosition();
			this.NavigationCacheMode = NavigationCacheMode.Required;
			mySlider.Value = MapControl.ZoomLevel;
		}

		private async void GetCoordinatesOnCurrentlyPosition()
		{
			Geolocator myGeolocator = new Geolocator();
			Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
			Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
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
			MapControl.MapServiceToken = "MapServiceToken";
		}

		private async void GetWeatherOnCurrentlyPosition(double latitude, double longitude)
		{
			var client = new HttpClient();
			var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude));
			var result = await response.Content.ReadAsStringAsync();

			try
			{
				var res = JsonConvert.DeserializeObject<RootObject>(result);
				if (res != null)
				{
					ProgressRingOnCurrentlyPosition.IsActive = false;
					txtBlock.Text = string.Join("\n", res.name + ", " + res.sys.country, CalculateKelvinToCelcius(res.main.temp) + " *C");
					TglButton.Visibility = Visibility.Visible; // pokaż przycisk po wczytaniu danych
					foreach (var item in res.weather)
					{
						txtBlock.Text += "\n" + item.main + "\n" + item.description;
						myImage.Source = new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + item.icon.ToString() + ".png"));
					}
					geolocation.Text = "Y: " + latitude.ToString() + "\t" + "X: " + longitude.ToString();
					AddMapIcon(latitude, longitude);
					TextBlockTimeFromPosition.Text = UnixTimeStampToDateTime(res.dt).ToString();
				}
				else
				{
					MessageDialog msg = new MessageDialog("cos nie tak");
					msg.ShowAsync();
				}
			}
			catch (Exception ex)
			{
				MessageDialog msg = new MessageDialog(ex.Message);
				msg.ShowAsync();
			}

		}

		private void AddMapIcon(double latitude, double longitude)
		{
			MapIcon MapIcon1 = new MapIcon();
			MapIcon1.Location = new Geopoint(new BasicGeoposition()
			{
				Latitude = 47.620,
				Longitude = -122.349
			});
			MapIcon1.NormalizedAnchorPoint = new Point(2, 2);
			MapIcon1.Title = "You are here!";
			MapControl.MapElements.Add(MapIcon1);
		}

		private string CalculateKelvinToCelcius(double Kelvin)
		{
			double x = Kelvin - 273.15;
			return x.ToString("N");
		}

		private void MapControl_OnMapTapped(MapControl sender, MapInputEventArgs args)
		{
			ShowWeather.Text = "";
			ShowPosition.Text = "";
			myImageToSelectedWeather.Visibility = Visibility.Collapsed;
			ProgressRingForSelectedWeather.IsActive = true;
			ProgressRingForSelectedWeather.Background = new SolidColorBrush(Colors.SlateGray);
			var latitude = args.Location.Position.Latitude;
			var longitude = args.Location.Position.Longitude;
			GetWeatherFromCoordinates(latitude, longitude);
			Pivot.SelectedIndex = 2;
		}

		private async void GetWeatherFromCoordinates(double latitude, double longitude)
		{
			var client = new HttpClient();
			var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude));
			var result = await response.Content.ReadAsStringAsync();

			try
			{
				var res = JsonConvert.DeserializeObject<RootObject>(result);
				if (res != null)
				{
					ProgressRingForSelectedWeather.IsActive = false;
					myImageToSelectedWeather.Visibility = Visibility.Visible;
					ShowWeather.Text = string.Join("\n", res.name + ", " + res.sys.country,
					CalculateKelvinToCelcius(res.main.temp) + " *C");

					foreach (var item in res.weather)
					{
						ShowWeather.Text += "\n" + item.main + "\n" + item.description;
						myImageToSelectedWeather.Source =
							new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + item.icon.ToString() + ".png"));
					}

					ShowPosition.Text = "Y: " + latitude + "\n" + "X: " + longitude;
					//AddMapIcon(latitude, longitude);
					TextBlockTimeForSelectedWeather.Text = "Sunrise: " + UnixTimeStampToDateTime(res.sys.sunrise).ToString();
				}
				else
				{
					MessageDialog msg = new MessageDialog("cos nie tak");
					msg.ShowAsync();
				}

			}
			catch (Exception ex)
			{
				MessageDialog msg = new MessageDialog(ex.Message);
				msg.ShowAsync();
			}

		}

		private void MySlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			if (MapControl != null)
			{
				MapControl.ZoomLevel = e.NewValue;
			}
		}

		public static string UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime.ToString("D");
		}

		private async void TglButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (TglButton.IsChecked == true)
			{
				TglButton.IsEnabled = false;
			}
			else
			{
				TglButton.IsChecked = true;
			}

			Geolocator myGeolocator = new Geolocator();
			Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
			Geocoordinate myGeocoordinate = myGeoposition.Coordinate;

			var client = new HttpClient();
			var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/find?lat=" + myGeocoordinate.Latitude + "&lon=" + myGeocoordinate.Longitude + "&cnt=3"));
			var result = await response.Content.ReadAsStringAsync();

			try
			{
				var res = JsonConvert.DeserializeObject<WeatherFor3Days>(result);
				if (res != null)
				{
					weatherFor3DaysList = res.list;
					for (int i = 0; i < res.list.Count; i++)
					{
						res.list[i].main.temp = res.list[i].main.temp - 273.15;
					}

					for (int i = 0; i < res.list.Count; i++)
					{
						res.list[i].weather[0].icon = "http://openweathermap.org/img/w/" + res.list[i].weather[0].icon + ".png";
					}

					listView.ItemsSource = weatherFor3DaysList;
				}
				else
				{
					MessageDialog msg = new MessageDialog("cos nie tak");
					msg.ShowAsync();
				}
			}
			catch (Exception ex)
			{
				MessageDialog msg = new MessageDialog(ex.Message);
				msg.ShowAsync();
			}
		}
	}
}
