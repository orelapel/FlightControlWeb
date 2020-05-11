using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class IntialLoation
    {
        private double longitude;
        private double latitude;
        private string dataTime;

        public IntialLoation(double longitude,double latitude,string dataTime)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.dataTime = dataTime;
        }
    }
}
