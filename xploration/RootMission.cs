using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xploration
{
    public class RootMission
    {
        public  int id { get; set; }
        public bool nasa { get; set; }
        public bool esa { get; set; }
        public bool jaxa { get; set; }
        public  int target { get; set; }
        public List<int> target_list { get; set; }
        public  int era { get; set; }
        public  string name { get; set; }
        public  string codename { get; set; }
        public  string hashed { get; set; }
        public  string image_url { get; set; }
        public  string launch_dates { get; set; }
        public  string twitter { get; set; }
        public  string fb_page { get; set; }
    }
}
