using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        [JsonProperty("passengers")]
        public int Passengers { get; set; }

        [JsonProperty("company_name")]
        public string Company_Name { get; set; }

        [JsonProperty("initial_location")]
        public InitialLocation Initial_Location { get; set; }

        [JsonProperty("segments")]
        public List<Segment> Segments { get; set; }

        public FlightPlan()
        {
            Passengers = -1;
        }
    }
}
