using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xploration
{
    class RootPlanet
    {
        public  int id { get; set; }
        public  string name { get; set; }
        public  string slug { get; set; }
        public  int body_type { get; set; }
        public  string image_url { get; set; }
        public  string characteristics { get; set; }
        public  string curiosities { get; set; }
        public  string sim_related { get; set; }
        public  bool use_in_sim { get; set; }
    }
}
