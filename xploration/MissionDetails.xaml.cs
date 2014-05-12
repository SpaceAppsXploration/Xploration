using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System.Diagnostics;

namespace xploration
{
    public partial class MissionDetails : PhoneApplicationPage
    {
        public MissionDetails()
        {
            InitializeComponent();
            DictionaryDetails_onCreate();
            RetrievingDetails();
            VisualizeDetails();
        }

        //creating a dictionary of a pair <int, string> to associate the details_type to a correspective item
        public Dictionary<int, string> dictionary_details = new Dictionary<int, string>();
        //creating a dictionary of a pair <int, string> to associate the era to a correspective item
        public Dictionary<int, string> dictionary_eras = new Dictionary<int, string>();
        
        public void DictionaryDetails_onCreate()
        {
            dictionary_details.Add(1, "goal");
            dictionary_details.Add(2, "accomplishment");
            dictionary_details.Add(3, "read_more");
            dictionary_details.Add(4, "mission_link");
            dictionary_details.Add(5, "event");
            dictionary_details.Add(6, "fact");
            dictionary_details.Add(7, "status");
            dictionary_details.Add(8, "article");
            dictionary_details.Add(9, "pubblications");
            dictionary_details.Add(10, "archive_link");
            dictionary_details.Add(11, "news");

            dictionary_eras.Add(0, "concept");
            dictionary_eras.Add(1, "past");
            dictionary_eras.Add(2, "present");
            dictionary_eras.Add(3, "future");
        }

        //retrieving details from the id selected by the user initializing a public object
        DetailsMain details_main = new DetailsMain();

        public void RetrievingDetails()
        {
            //retrieving data from the storage
            IsolatedStorageSettings detailSettings = IsolatedStorageSettings.ApplicationSettings;
            List<RootMission> missionList = (List<RootMission>)detailSettings["missionList"];
            int id_sel = Convert.ToInt32(detailSettings["search_missionList_id"]);

            foreach (RootMission rootmission in missionList)
                if (rootmission.id == id_sel)
                {
                    details_main.root_m = rootmission;
                    break;
                }
            RetrievingDetailsWeb();
        }

        //Downloading  details list
        public void RetrievingDetailsWeb()
        {
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            string site = "http://www.spacexplore.it/api/missions/details/" + details_main.root_m.id.ToString();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_detail_DownloadStringCompleted);
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

        private void client_detail_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
            try
            {
                //deserializing datas and saving them
                List<RootDetail> d_list = JsonConvert.DeserializeObject<List<RootDetail>>(e.Result);
                details_main.root_d_list = d_list;
                //MISSION
                List<RootDetail> eventOrFacts = details_main.root_d_list.OrderBy(x => x.detail_type).ToList();
               

                int i = 0;
                while (i  < eventOrFacts.Count())
                {
                    if (eventOrFacts[i].detail_type == 3)
                         eventOrFacts.RemoveAt(i);
                    if (eventOrFacts[i].detail_type == 4)
                         eventOrFacts.RemoveAt(i);
                    if (eventOrFacts[i].detail_type == 7)
                         eventOrFacts.RemoveAt(i);
                    if (eventOrFacts[i].detail_type == 9)
                         eventOrFacts.RemoveAt(i);
                    if (eventOrFacts[i].detail_type == 10)
                         eventOrFacts.RemoveAt(i);
                    i++;
                }
                 EventOrFactOrArticleList.ItemsSource = eventOrFacts;
            }
            catch
            {
                if (MessageBox.Show("Okay, Houston, we've had a problem here... There is a problem on the internal database, please check it later") == MessageBoxResult.OK)
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
            }

            
        }


        //using the retrieved data to populate the page
        public void VisualizeDetails()
        {
            //retrieving data from the storage
            IsolatedStorageSettings detailSettings = IsolatedStorageSettings.ApplicationSettings;

            //ABOUT PAGE
            mission_name.Text = details_main.root_m.name.ToString();
            if (details_main.root_m.launch_dates != null)
            {
                string mission_launch = details_main.root_m.launch_dates.Substring(1, details_main.root_m.launch_dates.Length - 2);
                mission_launch_date.Text = mission_launch;
            }
            else
                mission_launch_date.Text = "unknown";

            mission_status.Text = dictionary_eras[details_main.root_m.era] + " mission";

            //list of targets
            List<RootPlanet> targetsList = new List<RootPlanet>();
            List<RootPlanet> rootplanetlist = (List<RootPlanet>)detailSettings["targetList"];
            if (details_main.root_m.target_list != null)
            {
                foreach (int i in details_main.root_m.target_list)
                    targetsList.Add(rootplanetlist[i - 1]);
            }
            else
                targetsList.Add(rootplanetlist[details_main.root_m.target]);
            
            mission_targets.ItemsSource = targetsList;
       
            //picture
            if (!(details_main.root_m.image_url == "http://solarsystem.nasa.gov/missions/images/noimage-thm.gif"))
            {
                BitmapImage bmi = new BitmapImage(new Uri(details_main.root_m.image_url, UriKind.Absolute));
                mission_picture.Source = bmi;
                mission_picture.Height = 400;
                mission_picture.Width = 400;
            }
                        
           

        }
            
    }
}