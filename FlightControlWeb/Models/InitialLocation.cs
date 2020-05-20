using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class InitialLocation
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string  Date_Time { get; set; }

/*        public InitialLocation(double longitude,double latitude,DateTime dataTime)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.dataTime = dataTime;
        }*/

    }
}
