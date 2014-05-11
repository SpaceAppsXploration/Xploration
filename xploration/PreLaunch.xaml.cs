﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System.Diagnostics;

namespace xploration
{
    public partial class PreLaunch : PhoneApplicationPage
    {
        public PreLaunch()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Simulation.destination == null)
                destinationText.Text = "no destination selected";
            else
                destinationText.Text = Simulation.destination_complete;

            if (Simulation.mission == null)
                goalText.Text = "no mission selected";
            else
                goalText.Text = Simulation.mission_complete;
        }

        private void DestinationClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DestinationPage.xaml", UriKind.Relative));
        }

        private void GoalClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GoalPage.xaml", UriKind.Relative));
        }

        //only one click will cause the connection!
        public bool pressed = false;
        private void PreLaunch_Click(object sender, RoutedEventArgs e)
        {
            string simulation_pre_launch = Simulation.MakePreLaunch();
            if (simulation_pre_launch == null)
                MessageBox.Show("Ooops! You tried to start a mission without selecting the destination and/or the mission type. You're a bit confused lad!");
            else
                if(!pressed)
                {
                    PreLaunchSimulation(simulation_pre_launch);
                    pressed = false;
                    progBar.IsIndeterminate = true;
                }
        }

        //prelaunching the simulation... connection to the server
        public void PreLaunchSimulation(string parameters)
        {
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            string site = "http://www.spacexplore.it/api/simulation/" + parameters;
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_target_DownloadStringCompleted);
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

        private void client_target_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            pressed = false;
            if (e.Error != null)
                if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
            try
            {
                //deserializing response
                Debug.WriteLine(e.Result);
                Response response = JsonConvert.DeserializeObject<Response>(e.Result);
                if (response.code != 1)
                    //navigation to be performed
                    Debug.WriteLine("SUCCESS!");
                else
                    MessageBox.Show(response.type + "." + response.content + ".");
                progBar.IsIndeterminate = false;
            }
            catch { Debug.WriteLine("ERROR"); } 
        }
    }
}