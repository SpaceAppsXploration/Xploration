using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System.Diagnostics;

namespace xploration
{
    public partial class TechPage : PhoneApplicationPage
    {
        public TechPage()
        {
            InitializeComponent();
            InitializeTech();
        }

        //saving the checkbox for the payload
        List<CheckBox> payload_check = new List<CheckBox>();
        //saving the checkbox for the bus
        List<CheckBox> bus_check = new List<CheckBox>();

        public void InitializeTech()
        {
            //payload
            payload_check.Add(null);
            payload_check.Add(p1);
            payload_check.Add(p2);
            payload_check.Add(p3);
            payload_check.Add(p4);
            payload_check.Add(p5);

            //bus
            bus_check.Add(null);
            bus_check.Add(c1);
            bus_check.Add(c2);
            bus_check.Add(c3);
            bus_check.Add(c4);
            bus_check.Add(c5);
            bus_check.Add(c6);
            bus_check.Add(c7);
            bus_check.Add(c8);
            bus_check.Add(c9);
            bus_check.Add(c10);
            bus_check.Add(c11);
            bus_check.Add(c12);
            bus_check.Add(c13);
            bus_check.Add(c14);
            bus_check.Add(c15);
            bus_check.Add(c16);

            //mission information launch pivot
            destinationText.Text = Simulation.destination_complete;
            missionText.Text = Simulation.mission_complete;
        }

        //only one of the bus item option can be selected!
        private void CheckedBus(object sender, RoutedEventArgs e)
        {
            int index_sender = bus_check.IndexOf((sender as CheckBox));
            if (index_sender % 2 == 0)
            {
                if (bus_check[index_sender - 1].IsChecked == true)
                    bus_check[index_sender - 1].IsChecked = false;
            }
            else
            {
                if (bus_check[index_sender + 1].IsChecked == true)
                    bus_check[index_sender + 1].IsChecked = false;
            }
        }

        //simulation final launching
        public bool pressed = false;
        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            LaunchSimulationPrepare();
            if (Simulation.payload.Count == 0 && Simulation.bus.Count == 0)
                MessageBox.Show("Warning: your mission has no tecnical support.");
            else
                if (!pressed)
                {
                    LaunchSimulation();
                    pressed = true;
                    progBar.IsIndeterminate = true;
                }
        }

        public void LaunchSimulationPrepare()
        {
            Simulation.payload = new List<Instrument>();
            Simulation.bus = new List<BusInstrument>();

            for (int i = 1; i < payload_check.Count; i++ )
            {
                if (payload_check[i].IsChecked == true)
                {
                    Instrument instrument = new Instrument();
                    instrument.inst = payload_check[i].Tag.ToString();
                    instrument.used = true;

                    Simulation.payload.Add(instrument);
                }
            }

            for (int j = 1; j < bus_check.Count; j++)
           {
               if (bus_check[j].IsChecked == true)
               {
                   BusInstrument instrument = new BusInstrument();
                   instrument.bus_inst = bus_check[j].Tag.ToString();
                   instrument.active = true;

                   Simulation.bus.Add(instrument);
               }
           }
              
        }

        public void LaunchSimulation()
        {
            string parameters = Simulation.MakeSimulation();
            string site = "http://www.spacexplore.it/simulation/" + parameters;
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
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

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            pressed = false;
            if (e.Error != null)
            {
                if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
            }
            else
            {
                try
                {
                    //deserializing response
                    Response response = JsonConvert.DeserializeObject<Response>(e.Result);
                    if (response.code != 1)
                    {
                        finalLaunch.Visibility = Visibility.Collapsed;
                        successLaunch.Visibility = Visibility.Visible;
                    }
                    else
                        MessageBox.Show("The Tech Fellow says: '" + response.type + ". " + response.content + ".'");
                    progBar.IsIndeterminate = false;
                }
                 catch { Debug.WriteLine("ERROR"); }
            }
           
        }

        //performing back navigation
        private void launchNav_onTap(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBlock).Tag == (string)"pg")
            {
                Simulation.ClearAll();
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.RemoveBackEntry();
                NavigationService.GoBack();
            }
        }
    }
}