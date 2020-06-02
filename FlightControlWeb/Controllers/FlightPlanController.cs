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
            }
            catch
            {
                return BadRequest("There is no flight plan with this id");
            }
            return Ok(flightPlan);
        }

        // POST: api/FlightPlan
        [HttpPost]
        public ActionResult<string> AddFlightPlan([FromBody] FlightPlan flightPlan)
        {
            // Check if there is a problem in the json.
            if (flightPlan.Company_Name == null || flightPlan.Initial_Location == null
                || flightPlan.Segments == null
                || flightPlan.Initial_Location.Date_Time == null
                || flightPlan.Initial_Location.Latitude < -90
                || flightPlan.Initial_Location.Latitude > 90
                || flightPlan.Initial_Location.Longitude < -180
                || flightPlan.Initial_Location.Longitude > 180
                || flightPlan.Passengers < 0)
            {
                return BadRequest("this is not a valid flight plan");
            }
            foreach (Segment segment in flightPlan.Segments)
            {
                if (segment.Latitude < -90 || segment.Latitude > 90
                    || segment.Longitude < -180 || segment.Longitude > 180
                    || segment.Timespan_Seconds < 0)
                {
                    return BadRequest("this is not a valid flight plan");
                }
            }

            // If the json is OK - add this flight plan.
            try
            {
                flightManager.AddFlightPlan(flightPlan);
            }
            catch
            {
                return BadRequest("failed add flight plan");
            }
            return Ok("success add flight plan");
        }
    }
}
