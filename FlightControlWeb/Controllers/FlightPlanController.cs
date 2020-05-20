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
    public class FlightPlanController : ControllerBase
    {
        private FlightManager flightManager;

        public FlightPlanController(FlightManager flightManager)
        {
            this.flightManager = flightManager;
        }


        // GET: api/FlightPlan/5
        [HttpGet("{id}", Name = "Get")]
        public FlightPlan GetFlightPlan(string id)
        {
           return flightManager.GetFlightPlanById(id);
        }

        // POST: api/FlightPlan
        [HttpPost]
        public void Post([FromBody] FlightPlan flightPlan)
        {
            flightManager.AddFlightPlan(flightPlan);
        }
    }
}
