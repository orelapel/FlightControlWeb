﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public int Passengers { get; set; }
        public string company_name { get; set; }
        public InitialLocation Initial_Location { get; set; }
        public List<Segment> Segments { get; set; }
    }
}
