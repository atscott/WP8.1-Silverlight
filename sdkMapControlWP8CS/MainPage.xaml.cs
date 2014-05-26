/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Microsoft.Phone.Maps;
using WebScraper;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Shell;
using sdkMapControlWP8CS.Resources;


namespace sdkMapControlWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        const int MIN_ZOOM_LEVEL = 1;
        const int MAX_ZOOM_LEVEL = 20;
        const int MIN_ZOOMLEVEL_FOR_LANDMARKS = 16;

        ToggleStatus locationToggleStatus = ToggleStatus.ToggledOff;

        MapLayer locationLayer = null;
        MapLayer pushpinLayer = null;

        // Constructor.
        public MainPage()
        {

            InitializeComponent();

            // Create the localized ApplicationBar.
            BuildLocalizedApplicationBar();

        }

        // Placeholder code to contain the ApplicationID and AuthenticationToken
        // that must be obtained online from the Windows Phone Dev Center
        // before publishing an app that uses the Map control.
        private void sampleMap_Loaded(object sender, RoutedEventArgs e)
        {
            MapsSettings.ApplicationContext.ApplicationId = "MyLocator";
            MapsSettings.ApplicationContext.AuthenticationToken = "Ah0lUhUorbBEefBM-h-Ozp7sHI0qp4TMvbR7rvqT_gxWDTM8aF0i-7Jj_37FwfUY";
        }

        #region Event handlers for App Bar buttons and menu items

        void ToggleLocation(object sender, EventArgs e)
        {
            if (locationToggleStatus == ToggleStatus.ToggledOff)
            {
                UpdateCurrentLocation();
                locationToggleStatus = ToggleStatus.ToggledOn;
            }
            else
            {
                //myMap.Layers.Remove(locationLayer);
                locationLayer = null;
                locationToggleStatus = ToggleStatus.ToggledOff;
            }

        }


        void ZoomIn(object sender, EventArgs e)
        {
            if (myMap.ZoomLevel < MAX_ZOOM_LEVEL)
            {
                myMap.ZoomLevel++;
            }
        }

        void ZoomOut(object sender, EventArgs e)
        {
            if (myMap.ZoomLevel > MIN_ZOOM_LEVEL)
            {
                myMap.ZoomLevel--;
            }
        }

        async void GetMerchants(object sender, EventArgs e)
        {
            var merchants = await SmallShopsMerchantsScraper.GetMerchants();
            foreach (var merchant in merchants)
            {
                Geocoder geo = new Geocoder();
                var coordinates = await geo.GetCoordinates(merchant.Location);
                Pushpin pushpin = new Pushpin ();
                pushpin.Location = new GeoCoordinate(coordinates.Latitude, coordinates.Longitude);
                pushpin.Background = new SolidColorBrush(Colors.Black);
                pushpin.Content = merchant.Name;
                pushpin.Tag = merchant.Name;
                myMap.Children.Add(pushpin);
            }
        }

        #endregion

        #region Helper functions for App Bar button and menu item event handlers


        private async void UpdateCurrentLocation()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                this.myMap.Center = new GeoCoordinate(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);

                if (this.myMap.Children.Count != 0)
                {
                    var pushpin = myMap.Children.FirstOrDefault(p => (p.GetType() == typeof(Pushpin) && ((Pushpin)p).Tag == "locationPushpin"));

                    if (pushpin != null)
                    {
                        this.myMap.Children.Remove(pushpin);
                    }
                }

                Pushpin locationPushpin = new Pushpin();
                //locationPushpin.Style = this.Resources["PushpinStyle"] as Style;
                // locationPushpin.Template = this.Resources["PinTemplate"] as ControlTemplate;
                //  locationPushpin.Template = this.Resources["ImagePin"] as ControlTemplate;


                //Uri imgUri = new Uri("Images/MapPin2.png", UriKind.RelativeOrAbsolute);
                //BitmapImage imgSourceR = new BitmapImage(imgUri);
                //ImageBrush imgBrush = new ImageBrush() { ImageSource = imgSourceR };

                //locationPushpin.Content = new Rectangle()
                //{
                //    Fill = imgBrush,
                //    Height = 64,
                //    Width = 64
                //};

                locationPushpin.Background = new SolidColorBrush(Colors.Purple);
                locationPushpin.Content = "You are here";
                locationPushpin.Tag = "locationPushpin";
                locationPushpin.Location = new GeoCoordinate(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);
                this.myMap.Children.Add(locationPushpin);
                this.myMap.SetView(locationPushpin.Location, 18.0);
                //UserLocationMarker marker = (UserLocationMarker)this.FindName("UserLocationMarker");
                //marker.GeoCoordinate = new GeoCoordinate { Longitude = geoposition.Coordinate.Point.Position.Longitude, Latitude = geoposition.Coordinate.Point.Position.Latitude };
            }
            catch (Exception)
            {
                MessageBox.Show("Error getting location");
            }
        }
        #endregion

        // Create the localized ApplicationBar.
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.5;

            // Create buttons with localized strings from AppResources.
            // Toggle Location button.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/location.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarToggleLocationButtonText;
            appBarButton.Click += ToggleLocation;
            ApplicationBar.Buttons.Add(appBarButton);
            // Merchants button
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/landmarks.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarZoomInButtonText;
            appBarButton.Click += GetMerchants;
            ApplicationBar.Buttons.Add(appBarButton);
            // Zoom In button.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomin.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarZoomInButtonText;
            appBarButton.Click += ZoomIn;
            ApplicationBar.Buttons.Add(appBarButton);
            // Zoom Out button.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomout.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarZoomOutButtonText;
            appBarButton.Click += ZoomOut;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create menu items with localized strings from AppResources.
            // Toggle Location menu item.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarToggleLocationMenuItemText);
            appBarMenuItem.Click += ToggleLocation;
            ApplicationBar.MenuItems.Add(appBarMenuItem);

            // Zoom In menu item.
            appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarZoomInMenuItemText);
            appBarMenuItem.Click += ZoomIn;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
            // Zoom Out menu item.
            appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarZoomOutMenuItemText);
            appBarMenuItem.Click += ZoomOut;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private enum ToggleStatus
        {
            ToggledOff,
            ToggledOn
        }
    }
}
