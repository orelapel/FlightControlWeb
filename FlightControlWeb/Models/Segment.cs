using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        private double longitude;
        private double latitude;
        private int timespanSeconds;

        public Segment(double longitude,double latitude,int timespanSeconds)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.timespanSeconds = timespanSeconds;
        }
    }
}