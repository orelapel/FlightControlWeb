using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public string FlightId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Passengers { get; set; }
        public string Company_Name { get; set; }
        public string Date_Time { get; set; }
        public bool Is_External { get; set; }
    }
}
