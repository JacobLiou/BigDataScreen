using BigScreenDataShow.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigScreenDataShow.Models
{
    public class DailyPassData : PropertyChangedBase
    {
        private string testNum = "0";
        public string TestNum
        {
            get { return testNum; }
            set { testNum = value; SetPropertyChanged(); }
        }

        private string passNum = "0";
        public string PassNum
        {
            get { return passNum; }
            set { passNum = value; SetPropertyChanged(); }
        }

        private string passPercentage = "0";
        public string PassPercentage
        {
            get { return passPercentage; }
            set { passPercentage = value; SetPropertyChanged(); }
        }
    }
}
