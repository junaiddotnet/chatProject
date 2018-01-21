using BlogDal.Abstract;
using BlogDal.Concerete;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace blogProject.Views.Dashboard
{
    public class dashBoardmessage
    {
        public int uid { get; set; }
        public string name { get; set; }
        public string msg { get; set; }
        public string group { get; set; }
        public DateTime time { get; set; }
    }
    public class dashMessage
    {
       public string userName { get; set; }
       public int  commentId { get; set; }
       public string commentTxt { get; set; }
       public DateTime commentDate { get; set; }
       public int postId { get; set; }
       public string postName { get; set; }

    }
    [HubName("dashHub")]
    public class dashHub:Hub
    {
        public IpostRepository postRepository;
        private IgroupRepository groupRepositoty;

        public dashHub()
        {
            postRepository = new EFpostRepository();
            groupRepositoty = new EFgroupRepository();
        }
        public void joinBoard()
        {
            // Fetch the comments from Database ..
            var Comments = postRepository.comments.OrderByDescending(d => d.commentDate).Select(x => new {x.user.userName, x.commentId, x.commentTxt, x.commentDate, x.posts.postId, x.posts.postName }).Take(10);
            // Fetch the Most recent chat conversaton ..
            var groupChat = groupRepositoty.groupChats.OrderByDescending(d=>d.txtDate).
                Select(x => new { uid = x.users.userId, name = x.users.userName, msg = x.chatTxt, group = x.groups.groupName, time = x.txtDate }).Take(10);
            Clients.All.loadRecentComments(Comments);
            Clients.All.loadRecentChat(groupChat);
        }
       
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            return base.OnDisconnected();
        }
    } // End of class
}