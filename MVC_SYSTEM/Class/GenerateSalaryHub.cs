using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace MVC_SYSTEM.Class
{
    [HubName("GenerateSalaryHub")]
    public class GenerateSalaryHub : Hub
    {
        public void GetGenEnd(int var1, string hdrmsg, string msg, string status)
        {
            Clients.All.GenEnd(var1, hdrmsg, msg, status);
        }
    }
}