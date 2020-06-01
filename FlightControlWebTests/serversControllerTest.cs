using System;
using Xunit;
using FlightControlWeb;
using FlightControlWeb.Models;
using FlightControlWeb.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWebTests
{
    public class serversControllerTest
    {
        [Fact]
        public void AddServer_check()
        {
            FlightManager flightManager = new FlightManager();
            Server server1 = new Server { ServerId = "1", ServerURL = "http://rony1.atwebpages.com" };
            Server server2 = new Server { ServerId = "2", ServerURL = "http://rony2.atwebpages.com" };
            Server server3 = new Server { ServerId = "3", ServerURL = "http://rony3.atwebpages.com" };
            flightManager.AddServer(server1);
            flightManager.AddServer(server2);

            serversController sc = new serversController(flightManager);

            List<Server> servers = sc.GetAllServers().Value;

            bool check1 = servers.Contains(server1);
            bool check2 = servers.Contains(server3);

            Assert.True(check1);
            Assert.False(check2);
        }
    }
}
