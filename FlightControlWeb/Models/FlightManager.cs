using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


/// <summary>
/// https://localhost:44373/
/// </summary>

namespace FlightControlWeb.Models
{
    public class FlightManager
    {
        private static ConcurrentDictionary<string, FlightPlan> dicFlightPlans
            = new ConcurrentDictionary<string, FlightPlan>();
        private static ConcurrentDictionary<string, Server> dicServers
            = new ConcurrentDictionary<string, Server>();
        private static ConcurrentDictionary<string, Server> dicServersFId
            = new ConcurrentDictionary<string, Server>();

        public FlightManager()
        {
            AddServer(new Server { ServerId = "1", ServerURL = "http://rony10.atwebpages.com" });
            //AddServer(new Server { ServerId = "2", ServerURL = "http://rony2.atwebpages.com" });

            //List<Segment> segments1 = new List<Segment>()
            //{
            //    new Segment{Longitude=70,Latitude=70,Timespan_Seconds=950 },
            //    new Segment{Longitude=75,Latitude=75.34,Timespan_Seconds=550 },
            //    new Segment{Longitude=80,Latitude=80,Timespan_Seconds=1000 }
            //};
            //FlightPlan flightPlan1 = new FlightPlan
            //{
            //    Segments = segments1,
            //    Passengers = 120,
            //    Company_Name = "OrelFlightsLtd",
            //    Initial_Location = new InitialLocation { Longitude = 50, Latitude = 50, Date_Time = "2020-05-24T15:12:21Z" }
            //};
            //AddFlightPlan(flightPlan1);

            //List<Segment> segments2 = new List<Segment>()
            //{
            //    new Segment{Longitude=34.59,Latitude=35.94,Timespan_Seconds=550 },
            //    new Segment{Longitude=40,Latitude=41.39,Timespan_Seconds=550 },
            //    new Segment{Longitude=59.98,Latitude=59.99,Timespan_Seconds=550 }
            //};
            //FlightPlan flightPlan2 = new FlightPlan
            //{
            //    Segments = segments2,
            //    Passengers = 270,
            //    Company_Name = "EL-AL",
            //    Initial_Location = new InitialLocation { Longitude = 20, Latitude = 20, Date_Time = "2020-05-24T15:14:21Z" }
            //};
            //AddFlightPlan(flightPlan2);
            ////new InitialLocation { Longitude = 90,Latitude=90, Date_Time = "2020-12-26T23:56:21Z" }

            //List<Segment> segments3 = new List<Segment>()
            //{
            //    new Segment{Longitude=60,Latitude=60,Timespan_Seconds=700 },
            //    new Segment{Longitude=79,Latitude=80,Timespan_Seconds=550 },
            //    new Segment{Longitude=89,Latitude=89,Timespan_Seconds=550 }
            //};
            //FlightPlan flightPlan3 = new FlightPlan
            //{
            //    Segments = segments3,
            //    Passengers = 150,
            //    Company_Name = "NoaFlightLtd",
            //    Initial_Location = new InitialLocation { Longitude = 55, Latitude = 55, Date_Time = "2020-12-26T20:12:21Z" }
            //};
            //AddFlightPlan(flightPlan3);
        }
        public async virtual Task<List<Flight>> GetAllFlight(string relative_to, bool isExtrnal)
        {
            List<Flight> flights = new List<Flight>();
            DateTime relativeDate = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(relative_to));
            DateTime flightDate, lastFlightDate;
            List<Segment> flightSegments;
            // Go over all FlightPlans.
            foreach (KeyValuePair<string, FlightPlan> flightPlan in dicFlightPlans)
            {
                lastFlightDate = TimeZoneInfo.ConvertTimeToUtc
                    (Convert.ToDateTime(flightPlan.Value.Initial_Location.Date_Time));
                flightDate = TimeZoneInfo.ConvertTimeToUtc
                    (Convert.ToDateTime(flightPlan.Value.Initial_Location.Date_Time));
                flightSegments = flightPlan.Value.Segments;
                int j = 0;
                if (flightDate > relativeDate)
                {
                    // This Flight didnt started yet.
                    continue;
                }
                // Stop when we are in the segment or there are no more segments.
                while ((flightDate <= relativeDate) && (j < flightSegments.Count))
                {
                    lastFlightDate = flightDate;
                    // Each time- add the timespan seconds of the segments.
                    flightDate = flightDate.AddSeconds(flightSegments[j].Timespan_Seconds);
                    j++;
                }
                // If we are in the segment (the flight didnt finished yet).
                if (flightDate >= relativeDate)
                {
                    // Add flight to list.
                    Flight flight =
                        CreateCurrFlight(relativeDate, lastFlightDate, flightPlan, j, flightSegments);
                    flights.Add(flight);
                }
            }
            // If the user asked for external flights
            if (isExtrnal)
            {
                List<Flight> externalFlights = await RunExternalFlights(relative_to);
                flights.AddRange(externalFlights);
            }
            return flights;
        }
        protected virtual Flight CreateCurrFlight
            (DateTime relativeDate, DateTime lastFlightDate,
            KeyValuePair<string, FlightPlan> flightPlan,
            int numSeg, List<Segment> flightSegments)
        {
            double longitude1 = 0, longitude2, latitude1 = 0, latitude2, longitude3, latitude3;
            double secoInSegment, timeRatio, distance, midDistance;
            // Find how much time passed from segment till now.
            secoInSegment = relativeDate.Subtract(lastFlightDate).TotalSeconds;
            // Find the time ratio.
            timeRatio = secoInSegment / flightSegments[numSeg - 1].Timespan_Seconds;
            // Check if we are in the first segment.
            if (numSeg == 1)
            {
                // The last coordinate is from Initial_Location.
                longitude1 = flightPlan.Value.Initial_Location.Longitude;
                latitude1 = flightPlan.Value.Initial_Location.Latitude;
            }
            else
            {
                // The last coordinate is from last segment.
                longitude1 = flightSegments[numSeg - 2].Longitude;
                latitude1 = flightSegments[numSeg - 2].Latitude;
            }
            // The current segment's coordinates.
            longitude2 = flightSegments[numSeg - 1].Longitude;
            latitude2 = flightSegments[numSeg - 1].Latitude;
            // Linear interpolation
            distance = Math.Sqrt((Math.Pow(longitude2 - longitude1, 2)
                + Math.Pow(latitude2 - latitude1, 2)));
            midDistance = timeRatio * distance;
            latitude3 = latitude1 - ((midDistance) * (latitude1 - latitude2) / distance);
            longitude3 = longitude1 - ((midDistance) * (longitude1 - longitude2) / distance);
            // Create new Flight with the details we found.
            Flight flight = new Flight();
            flight.Longitude = longitude3;
            flight.Latitude = latitude3;
            flight.Flight_Id = flightPlan.Key;
            flight.Is_External = false;
            flight.Company_Name = flightPlan.Value.Company_Name;
            flight.Passengers = flightPlan.Value.Passengers;
            flight.Date_Time = flightPlan.Value.Initial_Location.Date_Time;
            return flight;
        }
        public virtual async Task<List<Flight>> RunExternalFlights
            (string relative_to)
        {
            List<Flight> flights = new List<Flight>();
            // Go over all servers
            foreach (KeyValuePair<string, Server> server in dicServers)
            {
                string request = server.Value.ServerURL + "/api/Flights?relative_to=" + relative_to;
                List<Flight> serverFlights = await GetFlightsFromServer(request);
                // If the server has no FlightPlan in this relative time.
                if (serverFlights == null)
                {
                    continue;
                }
                // Add the external flights ids to dictionary.
                AddFlightIdToDic(serverFlights, server.Value);
                ChangeFlightToExternal(serverFlights);
                // Add them to list.
                flights.AddRange(serverFlights);
            }
            return flights;
        }
        protected virtual void AddFlightIdToDic(List<Flight> serverFlights, Server server)
        {
            // Check for each flight in this server if it already exist in dictionary.
            foreach (Flight flight in serverFlights)
            {
                if (!dicServersFId.ContainsKey(flight.Flight_Id))
                {
                    // This flight not exist - so we add it to dictionary.
                    dicServersFId.TryAdd(flight.Flight_Id, server);
                }
            }
        }
        protected virtual async Task<List<Flight>> GetFlightsFromServer(string url)
        {
            string strResult = await SendRequestToServer(url);
            // Create the request.
            //string strurl = string.Format(url);
            //WebRequest requestObjGet = WebRequest.Create(strurl);
            //requestObjGet.Timeout = 100000;
            //requestObjGet.Method = "GET";
            //HttpWebResponse responseObjGet = null;

            //// Get the response from server.
            //try
            //{
            //    responseObjGet = (HttpWebResponse)await requestObjGet.GetResponseAsync();
            //}
            //catch
            //{
            //    return null;
            //}
            ////WebRequest requestObjGet = WebRequest.Create(strurl);
            ////requestObjGet.Method = "GET";
            ////HttpWebResponse responseObjGet = null;
            ////// Get the response from server.
            ////responseObjGet = (HttpWebResponse)await requestObjGet.GetResponseAsync();


            //// Return response to string (json).
            //string strResult = null;
            //using (Stream stream = responseObjGet.GetResponseStream())
            //{
            //    StreamReader sr = new StreamReader(stream);
            //    strResult = sr.ReadToEnd();
            //    sr.Close();
            //}
            List<Flight> serverFlights;
            // Try to deserialize the jason to list of Flight.
            try
            {
                serverFlights = JsonConvert.DeserializeObject<List<Flight>>(strResult);
            }
            catch
            {
                // If it failed- return null.
                return null;
            }
            return serverFlights;

        }
        protected virtual async Task<string> SendRequestToServer(string url)
        {
            // Create the request.
            string strurl = string.Format(url);
            WebRequest requestObjGet = WebRequest.Create(strurl);
            requestObjGet.Timeout = 100000;
            requestObjGet.Method = "GET";
            HttpWebResponse responseObjGet = null;

            // Get the response from server.
            try
            {
                responseObjGet = (HttpWebResponse)await requestObjGet.GetResponseAsync();
            }
            catch
            {
                return null;
            }

            // Return response to string (json).
            string strResult = null;
            using (Stream stream = responseObjGet.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                strResult = sr.ReadToEnd();
                sr.Close();
            }
            return strResult;
        }

        protected virtual void ChangeFlightToExternal(List<Flight> flights)
        {
            // Go over all Flights.
            foreach (Flight flight in flights)
            {
                // Change this field to true.
                flight.Is_External = true;
            }
        }
        public async Task<FlightPlan> GetFlightPlanById(string id)
        {
            // Check if this flight is internal.
            if (dicFlightPlans.ContainsKey(id))
            {
                return dicFlightPlans[id];
            }
            if (dicServersFId.ContainsKey(id))
            {
                //Send request to this server.
                string request = dicServersFId[id].ServerURL + "/api/FlightPlan/" + id;
                FlightPlan serverFlightPlan = await GetFlightPlanFromServer(request);
                // Check if this flight exist in this server.
                if (serverFlightPlan.Company_Name != null)
                {
                    return serverFlightPlan;
                }
            }

            //// Go over all external servers.
            //foreach (KeyValuePair<string, Server> server in dicServers)
            //{
            //    // Send request to this server.
            //    string request = server.Value.ServerURL + "/api/FlightPlan/" + id;
            //    FlightPlan serverFlightPlan = await GetFlightPlanFromServer(request);
            //    // Check if this flight exist in this server.
            //    if (serverFlightPlan.Company_Name != null)
            //    {
            //        return serverFlightPlan;
            //    }
            //}


            // There is no FlightPlan with this id (internal and external).
            return null;
        }

        private async Task<FlightPlan> GetFlightPlanFromServer(string url)
        {
            string strResult = await SendRequestToServer(url);
            // Deserialize the json to FlightPlan object.
            FlightPlan flightPlan = JsonConvert.DeserializeObject<FlightPlan>(strResult);
            return flightPlan;
        }
        public FlightPlan AddFlightPlan(FlightPlan flightPlan)
        {
            // Create id to the new FlightPlan.
            Random rnd = new Random();
            int numberFlight = rnd.Next(1000, 9999);
            string nameFlight = flightPlan.Company_Name.Substring(0, 3);
            string id = nameFlight + numberFlight;
            // Add to FlightPlan dictionary.
            dicFlightPlans.TryAdd(id, flightPlan);
            return flightPlan;
        }

        public void DeleteFlight(string id)
        {
            // Try to remove the Flight with this id.
            FlightPlan flightPlan = dicFlightPlans[id];
            dicFlightPlans.TryRemove(id, out flightPlan);
        }
        public List<Server> GetAllServer()
        {
            // Create list of servers from the dictionary.
            List<Server> servers = new List<Server>();
            foreach (KeyValuePair<string, Server> server in dicServers)
            {
                servers.Add(server.Value);
            }
            return servers;
        }
        public void AddServer(Server s)
        {
            dicServers.TryAdd(s.ServerId, s);
        }
        public void DeleteServerByID(string id)
        {
            // Find the server with this id and delete him.
            Server server = dicServers[id];
            dicServers.TryRemove(id, out server);
        }
    }
}