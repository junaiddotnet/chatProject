using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace blogProject.Views.Chat
{
    [HubName("newHub")]
    public class NewHub:Hub
    {
        public NewHub()
        {

        }
        public void joinChatt()
        {

        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}