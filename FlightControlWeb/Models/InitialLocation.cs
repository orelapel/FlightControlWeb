using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class InitialLocation
    {
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("date_time")]
        public string  DateTime { get; set; }

/*        public InitialLocation(double longitude,double latitude,DateTime dataTime)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.dataTime = dataTime;
        }*/

    }
}
