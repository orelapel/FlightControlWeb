using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightManager
    {
        private static List<Flight> flights = new List<Flight>()
        {
            new Flight { FlightId = "orelApel", Longitude = 8.6, Latitude = 7.4 }
        };
        List<Flight> GetAllFlight()
        {
            return flights;
        }
        Flight GetFlightById(string id)
        {
            Flight flight = flights.Where(x => x.FlightId == id).FirstOrDefault();
            return flight;
        }
        void AddFlight(Flight f)
        {
            flights.Add(f);
        }
        void UpdateFlight(Flight f)
        {
            Flight flight = flights.Where(x => x.FlightId == f.FlightId).FirstOrDefault();
            flight.Latitude = f.Latitude;
            flight.Longitude = f.Longitude;
            flight.Passengers = f.Passengers;
            flight.CompanyName = f.CompanyName;
            flight.IsExternal = f.IsExternal;
            flight.DataTime = f.DataTime;
        }
        void DeleteFlight(string  id)
        {
            Flight flight = flights.Where(x => x.FlightId == id).FirstOrDefault();
            if (flight == null)
            {
                throw new Exception("fligth not found");
            }
            flights.Remove(flight);
        }

    }
}
