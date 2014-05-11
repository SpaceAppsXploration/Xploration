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

namespace xploration
{
    public partial class PlanetPage : PhoneApplicationPage
    {
        public PlanetPage()
        {
            InitializeComponent();

            Downloading.Visibility = Visibility.Visible;
            progBar.IsIndeterminate = true;
            physicsPivot.Visibility = Visibility.Collapsed;

            RetrievingPhysics();
        }


        //retrieving data from the server about planet physics
        public void RetrievingPhysics()
        {
            IsolatedStorageSettings planetSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!planetSettings.Contains("physicsList"))
            {
                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                string site = "http://www.spacexplore.it/api/physics/planets/";
                try
                {
                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                }
                catch
                {
                    if (MessageBox.Show("Okay, Houston, we've had a problem here... Your connection failed!") == MessageBoxResult.OK)
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                }
            }
            else
                Visualize();
        }
            

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //on successfullo connection, saving data
                IsolatedStorageSettings planetSettings = IsolatedStorageSettings.ApplicationSettings;
                List<RootPhysics> rootphysics = JsonConvert.DeserializeObject<List<RootPhysics>>(e.Result);
                planetSettings["physicsList"] = (List<RootPhysics>)rootphysics;
                Visualize();
            }
            else
                if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
        }

        //visualizing informations
        public void Visualize()
        {
            Downloading.Visibility = Visibility.Collapsed;
            progBar.IsIndeterminate = false;
            physicsPivot.Visibility = Visibility.Visible;


        }
    }
}