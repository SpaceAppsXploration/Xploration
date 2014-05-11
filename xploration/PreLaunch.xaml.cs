using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace xploration
{
    public partial class PreLaunch : PhoneApplicationPage
    {
        public PreLaunch()
        {
            InitializeComponent();
        }

        private void DestinationClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DestinationPage.xaml", UriKind.Relative));
        }

        private void GoalClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GoalPage.xaml", UriKind.Relative));
        }
    }
}