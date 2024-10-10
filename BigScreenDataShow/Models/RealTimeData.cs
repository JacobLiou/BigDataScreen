using BigScreenDataShow.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigScreenDataShow.Models
{
    public class RealTimeData : PropertyChangedBase
    {
        private string realTime = "0";

        public string RealTime
        {
            get { return realTime; }
            set { realTime = value; SetPropertyChanged(); }
        }
    }
}
