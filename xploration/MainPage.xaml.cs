﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using xploration.Resources;

namespace xploration
{
    public partial class MainPage : PhoneApplicationPage
    {


        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Simulation.ClearAll();
        }

        private void AcceptChallenge(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PreLaunch.xaml", UriKind.Relative));
        }

        private void eXplore(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MissionsAllin.xaml", UriKind.RelativeOrAbsolute));
        }

        //email sender for the support service
        private void emailSupportSend_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.To = "dev.xploration@outlook.com";
            emailComposeTask.Subject = "XplorationApp issue";
            emailComposeTask.Body = "";

            emailComposeTask.Show();
        }
    }
}