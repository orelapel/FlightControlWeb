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
            List<Segment> segments1 = new List<Segment>()
            {
                new Segment{Longtitude=70,Latitude=70,TimespanSeconds=950 },
                new Segment{Longtitude=75,Latitude=75.34,TimespanSeconds=550 },
                new Segment{Longtitude=80,Latitude=80,TimespanSeconds=1000 }
            };
            FlightPlan flightPlan1 = new FlightPlan { Segments =segments1, Passengers = 120, CompanyName = "OrelFlightsLtd", InitialLocation = new InitialLocation { Longitude = 50, Latitude = 50, DateTime = "2020-12-26T21:57:21Z" } };
            AddFlightPlan(flightPlan1);



            List<Segment> segments2 = new List<Segment>()
            {
                new Segment{Longtitude=34.59,Latitude=35.94,TimespanSeconds=550 },
                new Segment{Longtitude=40,Latitude=41.39,TimespanSeconds=550 },
                new Segment{Longtitude=59.98,Latitude=59.99,TimespanSeconds=550 }
            };
            FlightPlan flightPlan2 = new FlightPlan { Segments = segments2,Passengers = 270, CompanyName = "EL-AL", InitialLocation = new InitialLocation { Longitude = 20, Latitude = 20, DateTime = "2020-12-26T23:56:21Z" } };
            AddFlightPlan(flightPlan2);
            //new InitialLocation { Longitude = 90,Latitude=90, Date_Time = "2020-12-26T23:56:21Z" }

            List<Segment> segments3 = new List<Segment>()
            {
                new Segment{Longtitude=60,Latitude=60,TimespanSeconds=700 },
                new Segment{Longtitude=79,Latitude=80,TimespanSeconds=550 },
                new Segment{Longtitude=89,Latitude=89,TimespanSeconds=550 }
            };
            FlightPlan flightPlan3 = new FlightPlan { Segments = segments3 ,Passengers = 150, CompanyName = "NoaFlightLtd", InitialLocation = new InitialLocation { Longitude = 55, Latitude = 55, DateTime = "2020-12-26T20:12:21Z" } };
            AddFlightPlan(flightPlan3);
            AddServer(new Server { ServerId="12344321", ServerURL = "https://localhost:44373" });

        }
        protected List<Flight> GetFlightsFromServer(string url)//object sender,EventArgs e,
        {
            string strurl = string.Format(url);
            WebRequest requestObjGet = WebRequest.Create(strurl);
            requestObjGet.Method = "GET";
            HttpWebResponse responseObjGet = null;
            responseObjGet =(HttpWebResponse) requestObjGet.GetResponse();

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
/*        private static List<Flight> flights = new List<Flight>()
        {
            new Flight { FlightId = "orelApel", Longitude = 8.6, Latitude = 7.4 }
        };
*/
        public List<Flight> GetAllFlight(string relative_to,bool isExtrnal)
        {
            List<Flight> flights = new List<Flight>();

            //long ticks = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day), Int32.Parse(hour), Int32.Parse(minute), Int32.Parse(second),
            //new CultureInfo("en-US", false).Calendar).Ticks;
            //DateTime dt3 = new DateTime(ticks);
            DateTime relativeDate = FromStringToDateTime(relative_to);
            DateTime flightDate;
            DateTime lastFlightDate;
            double secoInSegment;
            double relativeTime;
            double distance;
            double midDistance;
            double longtitude1, longtitude2, latitude1, latitude2,longtitude3,latitude3;
            List<Segment> flightSegments;
            foreach (KeyValuePair<string, FlightPlan> flightPlan in dicFlightPlans)
            {
                lastFlightDate = FromStringToDateTime(flightPlan.Value.InitialLocation.DateTime);
                flightDate = FromStringToDateTime(flightPlan.Value.InitialLocation.DateTime);
                flightSegments = flightPlan.Value.Segments;
                int j = 0;
                if (flightDate > relativeDate)
                {
                    continue;
                }
                while ((flightDate <= relativeDate) && (j < flightSegments.Count)) {
                    lastFlightDate = flightDate;
                    flightDate=flightDate.AddSeconds(flightSegments[j].TimespanSeconds);
                    j++;
                    
                }
                if (flightDate >= relativeDate)
                {
                    secoInSegment = relativeDate.Subtract(lastFlightDate).TotalSeconds;
                    relativeTime = secoInSegment / flightSegments[j - 1].TimespanSeconds;
                    if (j == 1)
                    {
                        longtitude1 =  flightPlan.Value.InitialLocation.Longitude;
                        latitude1 = flightPlan.Value.InitialLocation.Latitude;
                    }
                    else
                    {
                        longtitude1 = flightSegments[j - 2].Longtitude;
                        latitude1 = flightSegments[j - 2].Latitude;
                    }
                    longtitude2 = flightSegments[j - 1].Longtitude;
                    latitude2 = flightSegments[j - 1].Latitude;
                    distance = Math.Sqrt((Math.Pow(longtitude2 - longtitude1, 2) + Math.Pow(latitude2 - latitude1, 2)));
                    midDistance = relativeTime * distance;
                    latitude3 = latitude2 - ((midDistance) * (latitude2 - latitude1) / distance);
                    longtitude3 =longtitude2 - ((midDistance) * (longtitude2 - longtitude1) / distance);
                    Flight f = new Flight();
                    f.Longitude = longtitude3;
                    f.Latitude = latitude3;
                    f.FlightId = flightPlan.Key;
                    f.IsExternal = isExtrnal;
                    f.CompanyName = flightPlan.Value.CompanyName;
                    f.Passengers = flightPlan.Value.Passengers;
                    //f.Date_Time = flightDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    f.DateTime = relative_to;
                    flights.Add(f);
                }

            }
            if (isExtrnal)
            {
                foreach (KeyValuePair<string, Server> server in dicServers)
                {
/*                    WebRequest request = WebRequest.Create(server.Value.ServerURL+ "/api/FlightPlan");
                   // flights.Add();*/
                   string request= server.Value.ServerURL + "/api/Flights?relative_to="+ relative_to;
                    List<Flight> serverFlights=GetFlightsFromServer(request);
                    flights.AddRange(serverFlights);
                }
                    
            }
            return flights;  
        }


        public FlightPlan GetFlightPlanById(string id)
        {
            return dicFlightPlans[id];
        }

        public void AddFlightPlan(FlightPlan flightPlan)
        { 
            Random rnd = new Random();
            int numberFlight = rnd.Next(1000, 9999);
            string nameFlight = flightPlan.CompanyName.Substring(0, 3);
            string id = nameFlight + numberFlight;
            dicFlightPlans.TryAdd(id, flightPlan);
        }

        public void DeleteFlight(string  id)
        {
            FlightPlan flightPlan = dicFlightPlans[id];
/*            if (flightPlan == null)
            {
                throw new Exception("fligthPlan not found");
            }*/
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
        public DateTime FromStringToDateTime(string dateTime)
        {
            string year = "", month = "", day = "", hour = "", minute = "", second = "";
            int i = 0;
            while (dateTime[i] != '-')
            {
                year += dateTime[i];
                i++;
            }
            i++;
            while (dateTime[i] != '-')
            {
                month += dateTime[i];
                i++;
            }
            i++;
            while (dateTime[i] != 'T')
            {
                day += dateTime[i];
                i++;
            }
            i++;
            while (dateTime[i] != ':')
            {
                hour += dateTime[i];
                i++;
            }
            i++;
            while (dateTime[i] != ':')
            {
                minute += dateTime[i];
                i++;
            }
            i++;
            while (dateTime[i] != 'Z')
            {
                second += dateTime[i];
                i++;
            }
            return TimeZoneInfo.ConvertTimeToUtc(new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day), Int32.Parse(hour), Int32.Parse(minute), Int32.Parse(second)));
        }

    }
}