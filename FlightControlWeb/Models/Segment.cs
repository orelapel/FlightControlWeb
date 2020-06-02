using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("timespan_seconds")]
        public double Timespan_Seconds { get; set; }

        public Segment()
        {
            Longitude = 181;
            Latitude = 91;
            Timespan_Seconds = -1;
        }
    }
}