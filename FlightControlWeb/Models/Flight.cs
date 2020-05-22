using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [JsonProperty("flight_id")]
        public string Flight_Id { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("passengers")]
        public int Passengers { get; set; }
        [JsonProperty("company_name")]
        public string Company_Name { get; set; }
        [JsonProperty("date_time")]
        public string Date_Time { get; set; }
        [JsonProperty("is_external")]
        public bool Is_External { get; set; }
    }
}
