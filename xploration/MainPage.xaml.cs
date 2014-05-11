using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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

        private void AcceptChallenge(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PreLaunch.xaml", UriKind.Relative));
        }

        private void eXplore(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MissionsAllin.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}