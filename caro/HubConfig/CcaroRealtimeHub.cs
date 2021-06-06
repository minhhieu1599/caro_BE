using caro.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace caro.HubConfig
{
    public class CcaroRealtimeHub : Hub
    {
        public async Task UserOnline(List<User> users) => await Clients.All.SendAsync("user-online", users);
       
        /* public async Task UserOnline(List<User> users) 2 cách
        {
            await Clients.All.SendAsync("user-online", users);
        }*/
    }
}
