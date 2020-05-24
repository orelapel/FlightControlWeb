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
        private static ConcurrentDictionary<string, FlightPlan> dicFlightPlans = new ConcurrentDictionary<string, FlightPlan>();
        private static ConcurrentDictionary<string, Server> dicServers = new ConcurrentDictionary<string, Server>();
        
        public FlightManager()
        {
            AddServer(new Server { ServerId = "1", ServerURL = "http://ronyut3.atwebpages.com/ap2" });
            AddServer(new Server { ServerId = "2", ServerURL = "http://ronyut4.atwebpages.com/ap2" });

            List<Segment> segments1 = new List<Segment>()
            {
                new Segment{Longitude=70,Latitude=70,Timespan_Seconds=950 },
                new Segment{Longitude=75,Latitude=75.34,Timespan_Seconds=550 },
                new Segment{Longitude=80,Latitude=80,Timespan_Seconds=1000 }
            };
            FlightPlan flightPlan1 = new FlightPlan 
            { Segments =segments1, Passengers = 120, Company_Name = "OrelFlightsLtd", 
                Initial_Location = new InitialLocation { Longitude = 50, Latitude = 50, Date_Time = "2020-05-24T15:12:21Z" } };
            AddFlightPlan(flightPlan1);



            List<Segment> segments2 = new List<Segment>()
            {
                new Segment{Longitude=34.59,Latitude=35.94,Timespan_Seconds=550 },
                new Segment{Longitude=40,Latitude=41.39,Timespan_Seconds=550 },
                new Segment{Longitude=59.98,Latitude=59.99,Timespan_Seconds=550 }
            };
            FlightPlan flightPlan2 = new FlightPlan 
            { Segments = segments2,Passengers = 270, Company_Name = "EL-AL", 
                Initial_Location = new InitialLocation { Longitude = 20, Latitude = 20, Date_Time = "2020-05-24T15:14:21Z" } };
            AddFlightPlan(flightPlan2);
            //new InitialLocation { Longitude = 90,Latitude=90, Date_Time = "2020-12-26T23:56:21Z" }

            List<Segment> segments3 = new List<Segment>()
            {
                new Segment{Longitude=60,Latitude=60,Timespan_Seconds=700 },
                new Segment{Longitude=79,Latitude=80,Timespan_Seconds=550 },
                new Segment{Longitude=89,Latitude=89,Timespan_Seconds=550 }
            };
            FlightPlan flightPlan3 = new FlightPlan 
            { Segments = segments3 ,Passengers = 150, Company_Name = "NoaFlightLtd", 
                Initial_Location = new InitialLocation { Longitude = 55, Latitude = 55, Date_Time = "2020-12-26T20:12:21Z" } };
            AddFlightPlan(flightPlan3);
            //AddServer(new Server { ServerId="12344321", ServerURL = "https://localhost:44373" });

        }
        protected async Task<List<Flight>> GetFlightsFromServer(string url)
        {
            string strurl = string.Format(url);
            WebRequest requestObjGet = WebRequest.Create(strurl);
            requestObjGet.Method = "GET";
            HttpWebResponse responseObjGet = null;
            responseObjGet =(HttpWebResponse) await requestObjGet.GetResponseAsync();

            string strResult = null;
            using (Stream stream = responseObjGet.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                strResult = sr.ReadToEnd();
                sr.Close();
            }
            List<Flight> serverFlights = JsonConvert.DeserializeObject<List<Flight>>(strResult);
            return serverFlights;

        }

        public async Task<List<Flight>> GetAllFlight(string relative_to,bool isExtrnal)
        {
            List<Flight> flights = new List<Flight>();
            DateTime relativeDate = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(relative_to));
            DateTime flightDate, lastFlightDate;
            List<Segment> flightSegments;
            foreach (KeyValuePair<string, FlightPlan> flightPlan in dicFlightPlans)
            {
                lastFlightDate = TimeZoneInfo.ConvertTimeToUtc
                    (Convert.ToDateTime(flightPlan.Value.Initial_Location.Date_Time));
                flightDate = TimeZoneInfo.ConvertTimeToUtc
                    (Convert.ToDateTime(flightPlan.Value.Initial_Location.Date_Time));
                flightSegments = flightPlan.Value.Segments;
                int j = 0;
                if (flightDate > relativeDate) {
                    continue;
                }
                while ((flightDate <= relativeDate) && (j < flightSegments.Count)) {
                    lastFlightDate = flightDate;
                    flightDate=flightDate.AddSeconds(flightSegments[j].Timespan_Seconds);
                    j++;
                }
                if (flightDate >= relativeDate) {
                    Flight flight = 
                        CreateCurrFlight(relativeDate, lastFlightDate,flightPlan,j,flightSegments);
                    flights.Add(flight);
                }
            }
            if (isExtrnal) {
                await RunExternalFlights(relative_to, flights);
            }
            return flights;  
        }
        private async Task<List<Flight>> RunExternalFlights
            (string relative_to, List<Flight> flights)
        {
            foreach (KeyValuePair<string, Server> server in dicServers)
            {
                string request = server.Value.ServerURL + "/api/Flights?relative_to=" +relative_to;
                List<Flight> serverFlights = await GetFlightsFromServer(request);
                ChangeFlightToExternal(serverFlights);
                flights.AddRange(serverFlights);
            }
            return flights;
        }

        private Flight CreateCurrFlight
            (DateTime relativeDate, DateTime lastFlightDate, 
            KeyValuePair<string, FlightPlan> flightPlan,
            int numSeg ,List<Segment> flightSegments)
        {
            double longitude1 = 0, longitude2, latitude1 = 0, latitude2, longitude3, latitude3;
            double secoInSegment, relativeTime, distance, midDistance;
            secoInSegment = relativeDate.Subtract(lastFlightDate).TotalSeconds;
            relativeTime = secoInSegment / flightSegments[numSeg - 1].Timespan_Seconds;
            if (numSeg == 1)
            {
                longitude1 = flightPlan.Value.Initial_Location.Longitude;
                latitude1 = flightPlan.Value.Initial_Location.Latitude;
            }
            else
            {
                longitude1 = flightSegments[numSeg - 2].Longitude;
                latitude1 = flightSegments[numSeg - 2].Latitude;
            }
            longitude2 = flightSegments[numSeg - 1].Longitude;
            latitude2 = flightSegments[numSeg - 1].Latitude;
            distance = Math.Sqrt((Math.Pow(longitude2 - longitude1, 2)
                + Math.Pow(latitude2 - latitude1, 2)));
            midDistance = relativeTime * distance;
            latitude3 = latitude2 - ((midDistance) * (latitude2 - latitude1) / distance);
            longitude3 = longitude2 - ((midDistance) * (longitude2 - longitude1) / distance);
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

        private void ChangeFlightToExternal(List<Flight> flights)
        {
            foreach (Flight flight in flights)
            {
                flight.Is_External = true;
            }
        }


        public FlightPlan GetFlightPlanById(string id)
        {
            return dicFlightPlans[id];
        }

        public FlightPlan AddFlightPlan(FlightPlan flightPlan)
        { 
            Random rnd = new Random();
            int numberFlight = rnd.Next(1000, 9999);
            string nameFlight = flightPlan.Company_Name.Substring(0, 3);
            string id = nameFlight + numberFlight;
            dicFlightPlans.TryAdd(id, flightPlan);
            return flightPlan;
        }

        public void DeleteFlight(string  id)
        {
            FlightPlan flightPlan = dicFlightPlans[id];
            dicFlightPlans.TryRemove(id,out flightPlan);
        }
        //grt
        public List<Server> GetAllServer()
        {
            List<Server> servers = new List<Server>();
            foreach (KeyValuePair<string, Server> server in dicServers)
            {
                servers.Add(server.Value);
            }
            return servers;
        }
        //POST
        public void AddServer(Server s)
        {
            dicServers.TryAdd(s.ServerId,s);
        }
        //dELETE 
        public void DeleteServerByID(string  id)
        {
            Server server = dicServers[id];
            dicServers.TryRemove(id, out server);
        }
    }
}