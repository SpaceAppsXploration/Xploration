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
    public partial class MissionDetails : PhoneApplicationPage
    {
        public MissionDetails()
        {
            InitializeComponent();
            DictionaryDetails_onCreate();
        }

        //creating a dictionary of a pair <int, string> to associate the details_type to the corrispective item
        public Dictionary<int, string> dictionary_details = new Dictionary<int, string>();
        
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
        }
            
    }
}