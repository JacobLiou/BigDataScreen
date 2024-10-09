using BigScreenDataShow.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigScreenDataShow.Models
{
    public class AgeDailyData : PropertyChangedBase
    {
        private string g3 = "0";
        public string G3 {
            get { return g3; }
            set { g3 = value; SetPropertyChanged(); }
        } 

        private string g4 = "0";
        public string G4 
        {
            get { return g4; }
            set { g4 = value; SetPropertyChanged(); }
        }
     
        private string ebi = "0";

        public string EBI
        {
            get { return ebi; }
            set { ebi = value; SetPropertyChanged(); }
        }

        private string sum = "0";

        public string Sum
        {
            get { return sum; }
            set { sum = g3 + g4 + ebi; SetPropertyChanged(); }
        }
    }
}
