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
        public async Task<IEnumerable<Flight>> GetAllFlights (string relative_to)
        {
            // check if the request contains "sync_all"
            string request = Request.QueryString.Value;
            bool isExternal = request.Contains("sync_all");
            return await flightManager.GetAllFlight(relative_to, isExternal);
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public void DeleteFlight(string id)
        {
            flightManager.DeleteFlight(id);
        }
    }
}
