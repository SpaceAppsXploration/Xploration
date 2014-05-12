using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace xploration
{
    public partial class GoalPage : PhoneApplicationPage
    {
        public GoalPage()
        {
            InitializeComponent();
        }

        public int prev = -1;

        private void Goal_Click(object sender, RoutedEventArgs e)
        {
            if (prev >= 0)
            {
                (missionsParent.Children[prev] as Border).Background = new SolidColorBrush(Colors.Transparent);
            }
            prev = Convert.ToInt32(((sender as TextBlock).Parent as Border).Tag.ToString());
            ((sender as TextBlock).Parent as Border).Background = new SolidColorBrush(Color.FromArgb(127,0,0,0));
            chooseButton.Tag = (sender as TextBlock).Tag.ToString();

            Simulation.mission_complete = (sender as TextBlock).Text;
        }

        private void ChooseMission_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Simulation.mission = (sender as Button).Tag.ToString();
                NavigationService.GoBack();
            }
            catch
            {
                MessageBox.Show("No mission type selected");
                NavigationService.GoBack();
            }
        }
    }

}