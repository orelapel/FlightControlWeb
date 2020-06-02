using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FlightControlWebTests
{
    public class FlightManagerTest
    {
        private async Task<List<Flight>> ReturnAllFlight()
        {
            // Add two external flights.
            List<Flight> flights = await Task.Run(() => new List<Flight>());
            Flight flight1 = new Flight
            {
                Company_Name = "company1",
                Passengers = 10,
                Date_Time = "2020-05-31T12:22:21Z",
                Latitude = 30,
                Longitude = 20,
                Is_External = true,
                Flight_Id = "comp4498"
            };

            Flight flight2 = new Flight
            {
                Company_Name = "company2",
                Passengers = 20,
                Date_Time = "2020-05-31T12:24:21Z",
                Latitude = 40,
                Longitude = 50,
                Is_External = true,
                Flight_Id = "comp6193"
            };
            flights.Add(flight1);
            flights.Add(flight2);
            return flights;
        }
        [Fact]
        public void GetAllFlights_CheckIfReturnedTheCorrectFlights()
        {
            // Arrange
            Mock<FlightManager> mockFlightManager = new Mock<FlightManager>();

            // Add flight plans to mockFlightManager.
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
                Company_Name = "Swiss",
                Initial_Location = new InitialLocation
                { Longitude = 50, Latitude = 50, Date_Time = "2020-05-31T12:20:21Z" }
            };
            mockFlightManager.Object.AddFlightPlan(flightPlan1);

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
                Initial_Location = new InitialLocation
                { Longitude = 20, Latitude = 20, Date_Time = "2020-05-31T12:23:21Z" }
            };
            mockFlightManager.Object.AddFlightPlan(flightPlan2);

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
                Company_Name = "Arkia",
                Initial_Location = new InitialLocation
                { Longitude = 55, Latitude = 55, Date_Time = "2020-05-31T12:21:21Z" }
            };
            mockFlightManager.Object.AddFlightPlan(flightPlan3);

            // Return the hard method RunExternalFlights to be ReturnAllFlight.
            mockFlightManager.Setup(x => x.RunExternalFlights(It.IsAny<String>()))
                .Returns(ReturnAllFlight());

            // Act - call the method we test.
            mockFlightManager.CallBase = true;
            var flights = mockFlightManager.Object.GetAllFlight("2020-05-31T12:25:21Z", true)
                .Result;

            // Assert - check if the list we get is correct.
            Assert.Equal(5, flights.Count);
            Assert.Equal("comp4498", flights[3].Flight_Id);
            Assert.Equal("comp6193", flights[4].Flight_Id);
            Assert.False(flights[0].Is_External);
            Assert.True(flights[3].Is_External);
            Assert.Equal(20, flights[3].Longitude);
            Assert.Equal(30, flights[3].Latitude);
            Assert.Equal("company2", flights[4].Company_Name);
        }
    }
}
