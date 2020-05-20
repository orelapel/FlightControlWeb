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
        public List<Server> Get()
        {
            return flightManager.GetAllServer();
        }

        // POST: api/servers
        [HttpPost]
        public void Post([FromBody] Server server)
        {
            flightManager.AddServer(server);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            flightManager.DeleteServerByID(id);
        }
    }
}
