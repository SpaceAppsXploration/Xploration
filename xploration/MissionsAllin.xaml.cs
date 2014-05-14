using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Phone.Globalization;
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
    public partial class MissionsAllin : PhoneApplicationPage
    {
        public MissionsAllin()
        {
            InitializeComponent();

            progBar.IsIndeterminate = true;
            Downloading.Visibility = Visibility.Visible;
            //retrieving data from the server if it is necessary... and visualizing them
            Caching();
        }

        public void Caching()
        {
            //using an isolatedstorage istance, the app saves locally the mission list from the server
            IsolatedStorageSettings missionSettings = IsolatedStorageSettings.ApplicationSettings;
            
            //if there are no missions saved...
            if (!missionSettings.Contains("targetList"))
                target_recovery();
            if (!missionSettings.Contains("missionList"))
                mission_recovery();
            else if (missionSettings.Contains("missionList"))
                Visualize();
            
            //if there are mission_saved, the app checks the day and the month to refresh the local list if it is necessary...
            //note please that once a week the server is updated in this version...
            if (missionSettings.Contains("missionList"))
                if (missionSettings.Contains("mission_cache"))
                {
                    DateTime date = (DateTime)missionSettings["mission_cache"];
                    if (!(date.Month == DateTime.Now.Month))
                        mission_recovery();
                    else
                        if ((DateTime.Now.Day - date.Day) > 6)
                            mission_recovery();
                        else
                            Visualize();
                }

        }

        //Downloading target list
        public void target_recovery()
        {
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            string site = "http://www.spacexplore.it/api/targets/";
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
                    //deserializing datas and saving them
                    IsolatedStorageSettings missionSettings = IsolatedStorageSettings.ApplicationSettings;
                    List<RootPlanet> targetList = JsonConvert.DeserializeObject<List<RootPlanet>>(e.Result);
                    missionSettings["targetList"] = (List<RootPlanet>)targetList;
                }
                catch
                {
                    Debug.WriteLine("ERROR");
                }
            }            
        }



        //Downloading the mission list (all of them, from NASA ESA and JAXA)
        public void mission_recovery()
        {
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            string site = "http://www.spacexplore.it/api/missions/";
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_missions_DownloadStringCompleted);
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

        private void client_missions_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
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
                    IsolatedStorageSettings missionSettings = IsolatedStorageSettings.ApplicationSettings;
                    List<RootMission> missionList = JsonConvert.DeserializeObject<List<RootMission>>(e.Result);
                    //reducing the original list into a more efficient one to eliminate the duplicated objects 
                    List<RootMission> missionListNew = new List<RootMission>();
                    foreach (RootMission rootmission in missionList)
                        if (missionListNew.Count() != 0)
                        {
                            rootmission.target_list = new List<int>() { rootmission.target };
                            bool found = false;
                            foreach (RootMission rootmission_new in missionListNew)
                                if (rootmission.name == rootmission_new.name)
                                {
                                    rootmission_new.target_list.Add(rootmission.target);
                                    found = true;
                                }
                            if (!found) { missionListNew.Add(rootmission); }
                        }
                        else { missionListNew.Add(rootmission); }
                    missionSettings["missionList"] = (List<RootMission>)missionListNew;
                    Visualize();
                }
                catch
                {
                    if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                }

            }
        }
            
            
            


        //visualizing data from the storage
        public void Visualize()
        {
            IsolatedStorageSettings missionSettings = IsolatedStorageSettings.ApplicationSettings;
            
            //retrieving data already saved
            List<RootMission> missionList = (List<RootMission>)missionSettings["missionList"];

            //dividing the results by the agency
            List<RootMission> NASAList = new List<RootMission>();
            List<RootMission> ESAList = new List<RootMission>();
            List<RootMission> JAXAList = new List<RootMission>();
            
            foreach (RootMission root in missionList)
            {
                if (root.nasa == true)
                    NASAList.Add(root);
                if (root.esa == true)
                    ESAList.Add(root);
                if (root.jaxa == true)
                    JAXAList.Add(root);
            }

            
            //this sort the list by alphabetical order
            List<AlphaKeyGroup<RootMission>> DataSourceNASA = AlphaKeyGroup<RootMission>.CreateGroups(NASAList,
                                                      System.Threading.Thread.CurrentThread.CurrentUICulture,
                                                      (RootMission mission) => { return mission.name; }, true);

            List<AlphaKeyGroup<RootMission>> DataSourceESA = AlphaKeyGroup<RootMission>.CreateGroups(ESAList,
                                                      System.Threading.Thread.CurrentThread.CurrentUICulture,
                                                      (RootMission mission) => { return mission.name; }, true);

            List<AlphaKeyGroup<RootMission>> DataSourceJAXA = AlphaKeyGroup<RootMission>.CreateGroups(JAXAList,
                                                      System.Threading.Thread.CurrentThread.CurrentUICulture,
                                                      (RootMission mission) => { return mission.name; }, true);

            //in the end visualizing the results...
            missionListNASA.ItemsSource = DataSourceNASA;
            missionListESA.ItemsSource = DataSourceESA;
            missionListJAXA.ItemsSource = DataSourceJAXA;

            progBar.IsIndeterminate = false;
            Downloading.Visibility = Visibility.Collapsed;
            agenciesPivot.Visibility = Visibility.Visible;
        }


        //tapping on the selected mission i navigate to the relative image
        private void Navigate_Mission_Tap(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings missionSettings = IsolatedStorageSettings.ApplicationSettings;
            int id_tag = Convert.ToInt32((sender as StackPanel).Tag.ToString());
            missionSettings["search_missionList_id"] = id_tag;
            //after saved the id of the mission selected, navigation to the details page
            NavigationService.Navigate(new Uri("/MissionDetails.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}