using CaroOnline2021.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaroOnline2021.HubConfig
{
    public class CaroRealtimeHub : Hub
    {
        public async Task UserOnline(List<User> users) => await Clients.All.SendAsync("user-online", users);
    }
}
