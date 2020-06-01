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


        // GET: api/FlightPlan/"id"
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            FlightPlan flightPlan;
            try
            {
                flightPlan = await flightManager.GetFlightPlanById(id);
            } catch
            {
                return BadRequest("There is no flight plan with this id");
            }
            return Ok(flightPlan);
        }

        // POST: api/FlightPlan
        [HttpPost]
        public ActionResult<string> AddFlightPlan([FromBody] FlightPlan flightPlan)
        {
            try
            {
                flightManager.AddFlightPlan(flightPlan);
            } catch
            {
                return BadRequest("failed add flight plan");
            }
            return Ok("success add flight plan");
        }
    }
}
