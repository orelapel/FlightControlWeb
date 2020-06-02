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
    public class serversController : ControllerBase
    {
        private FlightManager flightManager;
        public serversController(FlightManager flightManager)
        {
            this.flightManager = flightManager;
        }

        // GET: api/servers
        [HttpGet]
        public ActionResult<List<Server>> GetAllServers()
        {
            List<Server> servers;
            try
            {
                servers = flightManager.GetAllServer();
            }
            catch
            {
                return BadRequest("Problem in GetAllServers");
            }
            return servers;
        }

        // POST: api/servers
        [HttpPost]
        public ActionResult<string> AddServer([FromBody] Server server)
        {
            // Check if there is a problem in the json.
            if (server.ServerId == null || server.ServerURL == null)
            {
                return BadRequest("this is not a valid server");
            }

            // If the json is OK - add this server.
            try
            {
                flightManager.AddServer(server);
            }
            catch
            {
                return BadRequest("Problem add this server");
            }
            return Ok("success add this server");
        }

        // DELETE: api/ApiWithActions/id
        [HttpDelete("{id}")]
        public ActionResult<string> DeleteServer(string id)
        {
            try
            {
                flightManager.DeleteServerByID(id);
            }
            catch
            {
                return BadRequest("Problem delete this server");
            }
            return Ok("success delete this server");
        }
    }
}
