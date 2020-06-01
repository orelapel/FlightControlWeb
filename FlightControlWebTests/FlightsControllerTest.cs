using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FlightControlWebTests
{
    public class FlightsControllerTest
    {
        [Fact]
        public async void AddFlightPlan_check()
        {
            FlightManager flightManager = new FlightManager();

            List<Segment> segments1 = new List<Segment>()
            {
                new Segment{Longitude=70,Latitude=70,Timespan_Seconds=950 },
                new Segment{Longitude=75,Latitude=75.34,Timespan_Seconds=550 },
                new Segment{Longitude=80,Latitude=80,Timespan_Seconds=1000 }
            };
            FlightPlan flightPlan1 = new FlightPlan
            {
                Segments = segments1,
                Passengers = 120,
                Company_Name = "OrelFlightsLtd",
                Initial_Location = new InitialLocation { Longitude = 50, Latitude = 50, Date_Time = "2020-05-24T15:12:21Z" }
            };
            flightManager.AddFlightPlan(flightPlan1);


            List<Segment> segments2 = new List<Segment>()
            {
                new Segment{Longitude=34.59,Latitude=35.94,Timespan_Seconds=550 },
                new Segment{Longitude=40,Latitude=41.39,Timespan_Seconds=550 },
                new Segment{Longitude=59.98,Latitude=59.99,Timespan_Seconds=550 }
            };
            FlightPlan flightPlan2 = new FlightPlan
            {
                Segments = segments2,
                Passengers = 270,
                Company_Name = "EL-AL",
                Initial_Location = new InitialLocation { Longitude = 20, Latitude = 20, Date_Time = "2020-05-24T15:14:21Z" }
            };
            flightManager.AddFlightPlan(flightPlan2);
            //new InitialLocation { Longitude = 90,Latitude=90, Date_Time = "2020-12-26T23:56:21Z" }

            List<Segment> segments3 = new List<Segment>()
            {
                new Segment{Longitude=60,Latitude=60,Timespan_Seconds=700 },
                new Segment{Longitude=79,Latitude=80,Timespan_Seconds=550 },
                new Segment{Longitude=89,Latitude=89,Timespan_Seconds=550 }
            };
            FlightPlan flightPlan3 = new FlightPlan
            {
                Segments = segments3,
                Passengers = 150,
                Company_Name = "NoaFlightLtd",
                Initial_Location = new InitialLocation { Longitude = 55, Latitude = 55, Date_Time = "2020-12-26T15:30:21Z" }
            };
            flightManager.AddFlightPlan(flightPlan3);
            FlightsController fc = new FlightsController(flightManager);
           
           // List<Flight> flights = await fc.GetAllFlights("2020-05-24T15:10:21Z");
            await fc.GetAllFlights("2020-05-24T15:10:21Z");

            List<Flight> listFlight = new List<Flight>();
            //listFlight.AddRange(flights.Value);

            //bool check1 = listFlight.Contains(flightPlan1);
            //bool check2 = listFlight.Contains(flightPlan3);

            //Assert.True(check1);
            //Assert.False(check2);
        }

    }
}
