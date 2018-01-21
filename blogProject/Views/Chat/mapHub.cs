using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using BlogDal.Abstract;
using BlogDal.Concerete;
using BlogDal;
using System.Web.Helpers;

namespace blogProject.Views.Chat
{

     class mmessage
    {
        public int uid { get; set; }
        public string name { get; set; }
        public string msg { get; set; }
        public int group { get; set; }
        public DateTime time { get; set; }
        public int receiverId { get; set; }
    }

    [HubName("mapHub")]
    public class mapHub : Hub
    {
        private static Dictionary<string, int> onlineConnection = new Dictionary<string, int>();
        private IgroupRepository groupRepositoty;
        private IuserRepository userRepository;
        public mapHub()
        {
            groupRepositoty = new EFgroupRepository();
            userRepository = new EFuserRepository();
        }

        public void joinChatt(User user)
        {
            //------ when user join chat we assign the user table a connection id and save it to Table
            userRepository.saveConnection(user.userId, Context.ConnectionId);
            // Now user SignInn we got the new user Location now we have to Assign the new Locations 
           // userRepository.saveGeoLocation(user.userId, user.lat, user.lng);
            // ---------- this method is being called from client when clint first load chat app 
            Groups.Add(Context.ConnectionId, "Main");
            // --- When a user Join chat room then we have to make updation to all the clients conected to Hub by 
            // changing the status of user from Ofline to Online..
            var joinedUser = userRepository.users.Where(g => g.userId == user.userId).Select(d => new { userId = d.userId, name = d.userName, status = d.connectionId,lat=d.lat,lng=d.lng }).FirstOrDefault();

            loadgroupMembers();

            Clients.Others.setOnline(joinedUser);


        }


    
        public void broadcastMessage(User senderUser, string txtmessage, int groupId)
        {
            killConnections(); // It kills the connections wich are not in use anymore
            message tmessage = new message();
            groupChat chat = new groupChat();
            //---------------------------------
            chat.userId = senderUser.userId;
            chat.chatTxt = txtmessage;  // Prepare chat message to save in to Repoitory for Premanenet storage
            chat.txtDate = DateTime.Now;
            chat.groupId = groupId;
            //-----------------------------------
            tmessage.uid = senderUser.userId;
            tmessage.name = senderUser.name;
            tmessage.msg = txtmessage;  // Prepare message to broadcast to hub Clients in to required format
            tmessage.group = groupId;
            tmessage.time = DateTime.Now;
            tmessage.receiverId = 0;
            //----------------------------------
            groupRepositoty.addMessage(chat);

            Clients.Group(groupId.ToString()).displayMessage(tmessage);
        }

        public void join(int groupId, User prmuser)
        {
            groupUser user = groupRepositoty.groupUsers.Where(g => g.groupId == groupId && g.userId == prmuser.userId).FirstOrDefault();
            if (user == null)
            {
                groupRepositoty.joinGroup(new groupUser() { groupId = groupId, userId = prmuser.userId, activeStatus = "active" });
            }
            Groups.Add(Context.ConnectionId, groupId.ToString());
            loadgroupMessage(groupId); // Load the selected group or tab messages to caller chat window


            loadgroupMembers();

        }
        public void loadgroupMembers()
        {
            IList<string> StrconnectionIds = new List<string>();
           // onlineConnection.Clear(); // Clear the dictionary before it load the current active connections or Alive ones..

            var AllUsers = userRepository.users.Select(d => new { userId = d.userId, name = d.userName, status = d.connectionId,lat=d.lat,lng=d.lng }).ToList();
          
            Clients.Caller.loadMembers(AllUsers); // Call hub Client to load the users

        }
        // Kill the Dead client connections
        public void killConnections()
        {
            var gusers = userRepository.users.Where(g => g.connectionId != null).Select(d => new { userId = d.userId, connectionId = d.connectionId }).ToList();
            foreach (var usr in gusers)
            {
                if (onlineConnection.ContainsKey(usr.connectionId) == false && onlineConnection.Count > 0)
                {
                    setOffLine(usr.connectionId);
                }
            }
            // Kill the Dead connections..

        }

        public void cleanUpHub()
        {
            killConnections();
        }
        public void pingback(int userId)
        {
            // if client Ping back thats mean this connection is alive .. Now Make a List of Alive connections

            onlineConnection.Add(Context.ConnectionId, userId);
        }
        public void loadgroupMessage(int groupId)
        {
            //.. This statment fetches the chat messages of slected group and make it ready to be displayed in 
            // the caller chat windows .. by calling Client.caller..... 
            var groupChat = groupRepositoty.groupChats.Where(g => g.groupId == groupId).
                            Select(x => new { uid = x.users.userId, name = x.users.userName, msg = x.chatTxt, group = x.groups.groupName, time = x.txtDate });

            Clients.Caller.loadMessages(groupChat);
        }
        public void retriveUserChat(User selectedUser, User prmUser)
        {
            // we are going to check if selected user is alive(Online) or Not..

            // This Linq Query Retrive the communication between two selectedUsers...
            List<int> userss = new List<int> { selectedUser.userId, prmUser.userId };
            var userChat = from x in userRepository.chats
                           where userss.Contains(x.senderId) && userss.Contains(x.receiverId)
                           select (new { uid = x.sender.userId, name = x.sender.userName, msg = x.commentTxt, time = x.commentDate });

            Clients.Caller.loadUserChat(userChat);

        }
        public async Task sendMessage(string message, User senderUser, User receiverUser)
        {
            message tmessage = new message();
            string connectionId;
            userRepository.userMessage(new userChat
            {
                commentTxt = message,
                commentDate = DateTime.Now,
                senderId = senderUser.userId,
                receiverId = receiverUser.userId
            });
            //-----------------------------------
            tmessage.uid = senderUser.userId;
            tmessage.name = senderUser.name;
            tmessage.msg = message;  // Prepare message to broadcast to hub Clients in to required format
            tmessage.group = 0;
            tmessage.time = DateTime.Now;
            tmessage.receiverId = receiverUser.userId;
            //----------------------------------
            connectionId = userRepository.getConnction(receiverUser.userId);
            // Here we got the connection Id of selected user from the database table , we can ping selected User 
            // connection Id to Check if it is alive or dead...
            // ---------- Sill waiting for Ping function code to write ?? ---------------

            await Clients.Caller.displayUserMessage(tmessage);
            if (connectionId != null && connectionId != "") // check if user is onlne then transfer message oterwise message is saved offline for view
            {
                await Clients.Client(connectionId).displayUserMessage(tmessage);

            }

        }

        // We are going to use this method to check the user table and get the connection Id from that table and then check if the user of that hub connection is Dead 
        // or Stil Alive ....... We use some good techinique to write this method because we dont want the chat Room to slow while performing the requied functionalty 
        // thats why we use Async programming techinique .... 
        public async Task checkUserStatus()
        {

            await Clients.All.checkStatus();
        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            setOffLine(Context.ConnectionId);
            return base.OnDisconnected();
        }

        public void setOffLine(string ConnectionId)
        {
            int userId = userRepository.getUserId(ConnectionId); // Get the user Id by the current connection Id of current connected User

            userRepository.disConnection(ConnectionId);// When disconnect from Chat Hub . Remove connection Id from user Table ..

            var oflineUser = userRepository.users.Where(g =>  g.userId == userId).Select(d => new { userId = d.userId, name = d.userName, status = d.connectionId,lat=d.lat,lng=d.lng }).FirstOrDefault();

            Clients.All.setOfline(oflineUser);
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

    }
}