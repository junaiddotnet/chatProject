using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace blogProject.Views.Chat
{
    [HubName("postHub")]
    public class postHub:Hub
    {
        public postHub()
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