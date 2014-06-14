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
using System.Windows;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using sdkMapControlWP8CS.Resources;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using SmallShopsUnitedScraper;
using WebScraper;
using Microsoft.Phone.Maps.Toolkit;


namespace sdkMapControlWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        const int MinZoomLevel = 1;
        const int MaxZoomLevel = 20;
        const int MinZoomlevelForLandmarks = 16;

        ToggleStatus _locationToggleStatus = ToggleStatus.ToggledOff;

        MapLayer _locationLayer;
        MapLayer _pushpinLayer;

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
            if (_locationToggleStatus == ToggleStatus.ToggledOff)
            {
                UpdateCurrentLocation();
                _locationToggleStatus = ToggleStatus.ToggledOn;
            }
            else
            {
                myMap.Layers.Remove(_locationLayer);
                _locationLayer = null;
                _locationToggleStatus = ToggleStatus.ToggledOff;
            }

        }


        void ZoomIn(object sender, EventArgs e)
        {
            if (myMap.ZoomLevel < MaxZoomLevel)
            {
                myMap.ZoomLevel++;
            }
        }

        void ZoomOut(object sender, EventArgs e)
        {
            if (myMap.ZoomLevel > MinZoomLevel)
            {
                myMap.ZoomLevel--;
            }
        }

        async void GetMerchants(object sender, EventArgs e)
        {
            var merchants = await SmallShopsMerchantsScraper.GetMerchants();
            if (_pushpinLayer == null)
            {
                _pushpinLayer = new MapLayer();
                myMap.Layers.Add(_pushpinLayer);
            }
            foreach (var merchant in merchants)
            {
                var geo = new Geocoder();
                var coordinates = await geo.GetCoordinates(merchant.Location);

                if (FindName(merchant.Name + coordinates.Latitude + coordinates.Longitude) != null) continue;

                var pin = new Pushpin
                {
                    GeoCoordinate = new GeoCoordinate(coordinates.Latitude, coordinates.Longitude),
                    Name = merchant.Name + coordinates.Latitude + coordinates.Longitude,
                    Content = merchant.Name
                };

                var overlay = new MapOverlay
                {
                    Content = pin,
                    PositionOrigin = new Point(0.0, 1.0),
                    GeoCoordinate = new GeoCoordinate(coordinates.Latitude, coordinates.Longitude)
                };

                _pushpinLayer.Add(overlay);
            }
        }

        #endregion

        #region Helper functions for App Bar button and menu item event handlers


        private async void UpdateCurrentLocation()
        {
            var geolocator = new Geolocator { DesiredAccuracyInMeters = 50 };

            try
            {
                var geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));

                var marker = (UserLocationMarker)FindName("UserLocationMarker");
                marker.GeoCoordinate = new GeoCoordinate { Longitude = geoposition.Coordinate.Point.Position.Longitude, Latitude = geoposition.Coordinate.Point.Position.Latitude };
                var pushpin = (Pushpin)FindName("MyPushpin");
                pushpin.Content = "My Location";
                pushpin.GeoCoordinate = marker.GeoCoordinate;
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
            ApplicationBar = new ApplicationBar { Opacity = 0.5 };

            // Create buttons with localized strings from AppResources.
            // Toggle Location button.
            var appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/location.png", UriKind.Relative))
            {
                Text = AppResources.AppBarToggleLocationButtonText
            };
            appBarButton.Click += ToggleLocation;
            ApplicationBar.Buttons.Add(appBarButton);
            // Merchants button
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/landmarks.png", UriKind.Relative))
            {
                Text = AppResources.AppBarZoomInButtonText
            };
            appBarButton.Click += GetMerchants;
            ApplicationBar.Buttons.Add(appBarButton);
            // Zoom In button.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomin.png", UriKind.Relative))
            {
                Text = AppResources.AppBarZoomInButtonText
            };
            appBarButton.Click += ZoomIn;
            ApplicationBar.Buttons.Add(appBarButton);
            // Zoom Out button.
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomout.png", UriKind.Relative))
            {
                Text = AppResources.AppBarZoomOutButtonText
            };
            appBarButton.Click += ZoomOut;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create menu items with localized strings from AppResources.
            // Toggle Location menu item.
            var appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarToggleLocationMenuItemText);
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
