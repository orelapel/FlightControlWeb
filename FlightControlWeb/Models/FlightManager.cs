using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace FlightControlWeb.Models
{
    public class FlightManager
    {
        private static ConcurrentDictionary<string, FlightPlan> dicFlightPlans = new ConcurrentDictionary<string, FlightPlan>();
        private static ConcurrentDictionary<string, Server> dicServers = new ConcurrentDictionary<string, Server>();
        
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
                lastFlightDate = FromStringToDateTime(flightPlan.Value.Initial_Location.Date_Time);
                flightDate = FromStringToDateTime(flightPlan.Value.Initial_Location.Date_Time);
                flightSegments = flightPlan.Value.Segments;
                int j = 0;
                if (flightDate > relativeDate)
                {
                    continue;
                }
                while ((flightDate <= relativeDate) && (j < flightSegments.Count)) {
                    lastFlightDate = flightDate;
                    flightDate=flightDate.AddSeconds(flightSegments[j].Timespan_Seconds);
                    j++;
                    
                }
                if (flightDate >= relativeDate)
                {
                    secoInSegment = relativeDate.Subtract(lastFlightDate).TotalSeconds;
                    relativeTime = secoInSegment / flightSegments[j - 1].Timespan_Seconds;
                    if (j == 1)
                    {
                        longtitude1 =  flightPlan.Value.Initial_Location.Longitude;
                        latitude1 = flightPlan.Value.Initial_Location.Latitude;
                    }
                    else
                    {
                        longtitude1 = flightSegments[j - 2].Longitude;
                        latitude1 = flightSegments[j - 2].Latitude;
                    }
                    longtitude2 = flightSegments[j - 1].Longitude;
                    latitude2 = flightSegments[j - 1].Latitude;
                    distance = Math.Sqrt((Math.Pow(longtitude2 - longtitude1, 2) + Math.Pow(latitude2 - latitude1, 2)));
                    midDistance = relativeTime * distance;
                    latitude3 = latitude2 - ((midDistance) * (latitude2 - latitude1) / distance);
                    longtitude3 =longtitude2 - ((midDistance) * (longtitude2 - longtitude1) / distance);
                    Flight f = new Flight();
                    f.Longitude = longtitude3;
                    f.Latitude = latitude3;
                    f.FlightId = flightPlan.Key;
                    f.Is_External = isExtrnal;
                    f.Company_Name = flightPlan.Value.company_name;
                    f.Passengers = flightPlan.Value.Passengers;
                    //f.Date_Time = flightDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    f.Date_Time = relative_to;
                    flights.Add(f);
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
            string nameFlight = flightPlan.company_name.Substring(0, 3);
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