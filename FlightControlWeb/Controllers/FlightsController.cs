using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private FlightManager flightManager;

        public FlightsController(FlightManager flightManager)
        {
            this.flightManager = flightManager;
        }
        // GET: api/Flights?relative_to=<DATE_TIME>
        [HttpGet]
        public IEnumerable<Flight> GetAllFlights (string relative_to)
        {
            string request = Request.QueryString.Value;
            bool isExternal = request.Contains("sync_all");

            return flightManager.GetAllFlight(relative_to, isExternal);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            flightManager.DeleteFlight(id);
        }
    }
}
