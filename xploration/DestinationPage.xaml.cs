using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System.Diagnostics;

namespace xploration
{
    public partial class DestinationPage : PhoneApplicationPage
    {
        public DestinationPage()
        {
            InitializeComponent();

            Downloading.Visibility = Visibility.Visible;
            progBar.IsIndeterminate = true;
            destinationPivot.Visibility = Visibility.Collapsed;
            chooseButton.Visibility = Visibility.Collapsed;
            RetrievingPhysics();
        }

        public void RetrievingPhysics()
        {
            IsolatedStorageSettings destinationSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!destinationSettings.Contains("targetList"))
            {
                {
                    WebClient client = new WebClient();
                    client.UseDefaultCredentials = true;
                    string site = "http://www.spacexplore.it/api/targets/";
                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                    try
                    {
                        client.DownloadStringAsync(new Uri(site, UriKind.Absolute));
                    }
                    catch
                    {
                        if (MessageBox.Show("Okay, Houston, we've had a problem here... Your connection failed!") == MessageBoxResult.OK)
                            if (NavigationService.CanGoBack)
                                NavigationService.GoBack();
                    }

                }
            }
            else
            {
                Downloading.Visibility = Visibility.Collapsed;
                progBar.IsIndeterminate = false;
                destinationPivot.Visibility = Visibility.Visible;
            }
                 
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                    if (NavigationService.CanGoBack)
                       NavigationService.GoBack();
            try
                {
                //deserializing datas and saving them
                IsolatedStorageSettings destinationSettings = IsolatedStorageSettings.ApplicationSettings;
                List<RootPlanet> destinationList = JsonConvert.DeserializeObject<List<RootPlanet>>(e.Result);
                destinationSettings["targetList"] = (List<RootPlanet>)destinationList;

                
                Downloading.Visibility = Visibility.Collapsed;
                progBar.IsIndeterminate = false;
                destinationPivot.Visibility = Visibility.Visible;
            }
            catch
            {
                if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
            }

        }


        //visualizing informations
        private void Destination_Tap(object sender, RoutedEventArgs e)
        {
            //TODO to check simulation targets: they can change!
            Disclaimer.Visibility = Visibility.Collapsed;
            chooseButton.Visibility = Visibility.Visible;
            // visualization in the pivot page
            IsolatedStorageSettings destinationSettings = IsolatedStorageSettings.ApplicationSettings;
            List<RootPlanet> destinationList = (List<RootPlanet>)destinationSettings["targetList"];

            //retrieving the selected planet characteristics
            foreach(RootPlanet destination in destinationList)
               if ((sender as StackPanel).Tag.ToString() == destination.slug)
               {
                   //visualizing ad saving in the chooseButton tag!
                   BitmapImage bmi = new BitmapImage(new Uri(destination.image_url));
                   destinationImage.Source = bmi;

                   destinationText.Text = destination.characteristics;
                   destinationText.Visibility = Visibility.Visible;
                   chooseButton.Tag = destination.slug;

                   Simulation.destination_complete = destination.name;
               }
            destinationPivot.SelectedIndex = 1;
        }

        private void ChooseDestination_Click(object sender, RoutedEventArgs e)
        {
            Simulation.destination = (sender as Button).Tag.ToString();
            NavigationService.GoBack();
        }


    }
}