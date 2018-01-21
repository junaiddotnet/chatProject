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
using System.Text.RegularExpressions;

namespace blogProject.Views.Chat
{
    
    public class message
    {
        public int groupChatId { get; set; }
        public int uid { get; set; }
        public string name { get; set; }
        public string msg { get; set; }
        public int group { get; set; }
        public DateTime time { get; set; }
        public int receiverId { get; set; }
        public bool messageStatus { get; set; }
        public string txtType { get; set; }
        public string txtStatus { get; set; }
    }

    public class groupMembers
    {
        public int userId { get; set; }
        public string name { get; set; }
        public int groupId { get; set; }
        public string status { get; set; }
        public int unread { get; set; }
        public bool seen { get; set; }
        public string country { get; set; }
        public string activeStatus { get; set; }
    }


    [HubName("planeHub")]
    public class PlaneHub:Hub
    {
        private static Dictionary<string,int> onlineConnection= new Dictionary<string,int>();
        private IgroupRepository groupRepositoty;
        private IuserRepository userRepository;
        public PlaneHub()
        {
            groupRepositoty = new EFgroupRepository();
            userRepository = new EFuserRepository();
        }

        public void joinChatt(User user)
        {
            // Now Set the upper Tab options of Chat Controll andsetting the client optionssss
            //var TabOptions = groupRepositoty.groups.Select(c => new { id = c.groupId, option = c.groupName, nusers = 0 });

            var TabOptions = (from x in groupRepositoty.groups
                          
                          select new { id = x.groupId,option=x.groupName,messagecount=x.groupChats.Count(),unread=0 }
                          
                          ).Distinct();

                         
                             




            Clients.Caller.loadTabOptions(TabOptions);
            //------ when user join chat we assign the user table a connection id and save it to Table
            if (user != null)
            {
                userRepository.saveConnection(user.userId, Context.ConnectionId);


                // ---------- this method is being called from client when clint first load chat app 
                // so by default first Tab is automatically selected now we want to load the caller chat window with messages
                join(1, user);
                // Now check from Repository that this user is member of which group and then add the this List to caller or  hub 
                var userGroupList = groupRepositoty.groupUsers.Where(u => u.userId == user.userId).Select(c => new { groupName = c.groupId }).ToList();

                foreach (var grp in userGroupList)
                {
                    Groups.Add(Context.ConnectionId, grp.groupName.ToString()); // Add user group to hubbbb
                }


                // --- When a user Join chat room then we have to make updation to all the clients conected to Hub by 
                // changing the status of user from Ofline to Online..
                var joinedUser = groupRepositoty.groupUsers.Where(g => g.groupId == 1 && g.users.userId == user.userId).Select(d => new { userId = d.users.userId, name = d.users.userName, group = d.groupId, status = d.users.connectionId }).FirstOrDefault();

                Clients.Others.setOnline(joinedUser);
            } // End if Statment check if user is Not Null
            else
            {
                join(1, user);
            }
            

        }

        
        public void loadSelectedGroupUser(string groupName)
        {
        }
        public async Task broadcastLocationMessage(User senderUser, string txtmessage, int groupId)
        {
            Console.Write("dfadfad");
            message tmessage = new message();
            groupChat chat = new groupChat();
            var d = Clients.Caller.current;  // Temporary Checking only 
            //---------------------------------
            chat.userId = senderUser.userId;
            chat.chatTxt = txtmessage;  // Prepare chat message to save in to Repoitory for Premanenet storage
            chat.txtDate = DateTime.Now;
            chat.groupId = groupId;
            chat.txtType = "map";
            //-----------------------------------
            tmessage.uid = senderUser.userId;
            tmessage.name = senderUser.name;
            tmessage.msg = txtmessage;  // Prepare message to broadcast to hub Clients in to required format
            tmessage.group = groupId;
            tmessage.time = DateTime.Now;
            tmessage.receiverId = 0;
            tmessage.txtType = "map";
            //----------------------------------
            groupRepositoty.addMessage(chat);
            tmessage.groupChatId = chat.groupChatId;
            //-----------------------------------

            await Clients.Group(groupId.ToString()).displayMessage(tmessage);

            //---------------------------------------------Sending Message to Dashboard Hub -----------
            blogProject.Views.Dashboard.dashBoardmessage dmessage = new blogProject.Views.Dashboard.dashBoardmessage();

            var groupName = groupRepositoty.getName(groupId); //----- Fetch the Group name
            dmessage.uid = senderUser.userId;
            dmessage.name = senderUser.name;
            dmessage.msg = txtmessage;  // Prepare message to broadcast to hub Clients in to required format
            dmessage.time = DateTime.Now;

            dmessage.group = groupName;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<blogProject.Views.Dashboard.dashHub>();
            await hubContext.Clients.All.displayChatMessage(dmessage);
            //---------------------------------------------------
        }
        public async Task broadcastImageMessage(User senderUser, string txtmessage, int groupId)
        {
            message tmessage = new message();
            groupChat chat = new groupChat();
            var d = Clients.Caller.current;  // Temporary Checking only 
            //---------------------------------
            chat.userId = senderUser.userId;
            chat.chatTxt = txtmessage;  // Prepare chat message to save in to Repoitory for Premanenet storage
            chat.txtDate = DateTime.Now;
            chat.groupId = groupId;
            chat.txtType = "imagetxt";
            //-----------------------------------
            tmessage.uid = senderUser.userId;
            tmessage.name = senderUser.name;
            tmessage.msg = txtmessage;  // Prepare message to broadcast to hub Clients in to required format
            tmessage.group = groupId;
            tmessage.time = DateTime.Now;
            tmessage.receiverId = 0;
            tmessage.txtType = "imagetxt";
            //----------------------------------
            groupRepositoty.addMessage(chat);
            tmessage.groupChatId = chat.groupChatId;
            //-----------------------------------

           await Clients.Group(groupId.ToString()).displayMessage(tmessage);

            //---------------------------------------------Sending Message to Dashboard Hub -----------
            blogProject.Views.Dashboard.dashBoardmessage dmessage = new blogProject.Views.Dashboard.dashBoardmessage();

            var groupName = groupRepositoty.getName(groupId); //----- Fetch the Group name
            dmessage.uid = senderUser.userId;
            dmessage.name = senderUser.name;
            dmessage.msg = txtmessage;  // Prepare message to broadcast to hub Clients in to required format
            dmessage.time = DateTime.Now;

            dmessage.group = groupName;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<blogProject.Views.Dashboard.dashHub>();
            await hubContext.Clients.All.displayChatMessage(dmessage);
            //---------------------------------------------------
        }
        public async Task  broadcastMessage(User senderUser,string txtmessage, int groupId)
        {
            killConnections(); // It kills the connections wich are not in use anymore
            message tmessage = new message();
            groupChat chat = new groupChat();
            var d = Clients.Caller.current;  // Temporary Checking only 
            //---------------------------------
            chat.userId = senderUser.userId;
            //chat.chatTxt=Regex.Replace(txtmessage, @"\r\n?|\n", "<br />");
            //chat.chatTxt = txtmessage.Replace(@"\r\n", "<br/>");  // Prepare chat message to save in to Repoitory for Premanenet storage
            chat.chatTxt = txtmessage;
            chat.txtDate = DateTime.Now;
            chat.groupId = groupId;
            chat.txtType = "plaintxt";
            //-----------------------------------
            tmessage.uid = senderUser.userId;
            tmessage.name = senderUser.name;
            tmessage.msg = txtmessage;  // Prepare message to broadcast to hub Clients in to required format
            tmessage.group = groupId;
            tmessage.time = DateTime.Now;
            tmessage.receiverId = 0;
            //----------------------------------
             groupRepositoty.addMessage(chat);
            tmessage.groupChatId = chat.groupChatId;
            //-----------------------------------
           
           await Clients.Group(groupId.ToString()).displayMessage(tmessage);

            //---------------------------------------------Sending Message to Dashboard Hub -----------
           blogProject.Views.Dashboard.dashBoardmessage dmessage = new blogProject.Views.Dashboard.dashBoardmessage();

            var groupName = groupRepositoty.getName(groupId); //----- Fetch the Group name
            dmessage.uid = senderUser.userId;
            dmessage.name = senderUser.name;
            dmessage.msg = txtmessage;  // Prepare message to broadcast to hub Clients in to required format
            dmessage.time = DateTime.Now;

            dmessage.group = groupName ;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<blogProject.Views.Dashboard.dashHub>();
           await hubContext.Clients.All.displayChatMessage(dmessage);
            //---------------------------------------------------
        } // End of the Method 

        public async Task undoMarkDelete(User user,int groupId,int messageId)
        {
            message tmessage = new message();
            var msg= groupRepositoty.markAsUndo(messageId);
            //-----------------------------------
            tmessage.groupChatId = messageId;
            tmessage.uid = user.userId;
            tmessage.name = user.name;
            tmessage.msg = msg;  // Retrive the Original Mesage from Databse and broadcast to all the connected clients
            tmessage.group = groupId;
            tmessage.time = DateTime.Now;
            tmessage.receiverId = 0;
            tmessage.txtStatus = null;
            //----------------------------------
            /// --------  Send Message to all the connected Clients
            await Clients.Group(groupId.ToString()).markAsUndo(tmessage);
        }
        public async Task  deleteMessage( User user,int groupId,int messageId)
        {
            message tmessage = new message();
            groupRepositoty.markAsDelete(messageId);

            //-----------------------------------
            tmessage.groupChatId = messageId;
            tmessage.uid = user.userId;
            tmessage.name = user.name;
            tmessage.msg = "message Removed";  // Prepare message to broadcast to hub Clients in to required format
            tmessage.group = groupId;
            tmessage.time = DateTime.Now;
            tmessage.receiverId = 0;
            tmessage.txtStatus = "delete";
            //----------------------------------
            /// --------  Send Message to all the connected Clients
           await Clients.Group(groupId.ToString()).markAsDelete(tmessage);
        }
        public void join(int groupId,User prmuser)
        {
            if (prmuser!=null)
            {
                groupUser user = groupRepositoty.groupUsers.Where(g => g.groupId == groupId && g.userId == prmuser.userId).FirstOrDefault();
                if (user == null)
                {
                    groupRepositoty.joinGroup(new groupUser() { groupId = groupId, userId = prmuser.userId, activeStatus = "active" });
                }
            }
           
            Groups.Add(Context.ConnectionId, groupId.ToString());
            loadgroupMessage(groupId); // Load the selected group or tab messages to caller chat window
            
            if (prmuser!=null)
            {
                 loadgroupMembers(groupId, prmuser.userId);

            }

        }
        public async Task loadgroupMembers(int groupId,int userId)
        {
            List<int> userss = new List<int> { 2, 1002 };

            IList<string> StrconnectionIds = new List<string>();
            onlineConnection.Clear(); // Clear the dictionary before it load the current active connections or Alive ones..

            //var  groupUsers
            //   =  groupRepositoty.groupUsers.Where(g => g.groupId == groupId).Select(d => new  {userId = d.users.userId,name=d.users.userName,group=d.groupId,status=d.users.connectionId}).ToList();

           List<groupMembers> groupUsers = (from p in groupRepositoty.groupUsers
                              where p.groupId == groupId
                              select new groupMembers
                              {
                                  userId = p.userId,
                                  name=p.users.userName,
                                  groupId=p.groupId,
                                  status=p.users.connectionId,
                                  unread=p.userId,
                                  seen=true,
                                  country=p.users.country,
                                  activeStatus=p.users.status
                              }).ToList();
                              
            ///------
            int userCount = 0;
            foreach (var onlineUser in groupUsers)
            {
                if (onlineUser.status!=null)
                { // Check if record of status(Connection ID) in table of user is online then we are going to ping to check if user is alive or dead..

                    StrconnectionIds.Add(onlineUser.status);
                                
                }
                userss[0] = userId;
                userss[1] = onlineUser.userId;

                // Now find out the total of each user against The selected Log User
                // var total = (from x in userRepository.chats where userss.Contains(x.senderId) && userss.Contains(x.receiverId)  select x).Count();
                // below statment count the unread messages between two users
                var total = (from x in userRepository.chats.Where(c => c.senderId == onlineUser.userId && c.receiverId == userId && c.messageStatus==false) select x).Count();
                groupUsers[userCount].unread = total;
                userCount++;
            } // for loop ending 
          await Clients.Clients(StrconnectionIds).ping();
           

          await Clients.Caller.loadMembers(groupUsers);
            
        }
        // Change the Status of USer .  Online , Busy , Do not Disturb
        public void statusChange(string state, User Prmuser)
        {
            // Call the Repository and chage the status of user , What Ever Status user Selected ...
            userRepository.changeUserStatus(Prmuser.userId, state);
            Prmuser.activeStatus = state;
            //Clients.Caller.clientStatusChange(Prmuser);
            Clients.All.clientStatusChange(Prmuser);
        }
        // Kill the Dead client connections
        public void killConnections ()
        {
            var gusers = userRepository.users.Where(g=>g.connectionId!=null).Select(d => new { userId = d.userId, connectionId = d.connectionId }).ToList() ;
            foreach (var usr in gusers )
            {
                if (onlineConnection.ContainsKey(usr.connectionId)==false && onlineConnection.Count>0)
                {
                    setOffLine(usr.connectionId);
                }
            }
            // Kill the Dead connections..
           
        }

        public void cleanUpHub ()
        { 
            killConnections();
        }
        public void  pingback( int userId)
        {
            // if client Ping back thats mean this connection is alive .. Now Make a List of Alive connections
            // Now check the dictiinaory if connection is already there then do not add it again
            if (! onlineConnection.ContainsKey(Context.ConnectionId))
            {
                onlineConnection.Add(Context.ConnectionId, userId);

            }
        }
        public void loadgroupMessage (int groupId)
        {
            int skiptotal = 0;
            int pageLength = 30;
            skiptotal = groupRepositoty.GroupMessageTotal(groupId) - 30;
            if (skiptotal<0)
            {
                skiptotal = 1;
            }
            //.. This statment fetches the chat messages of slected group and make it ready to be displayed in 
            // the caller chat windows .. by calling Client.caller..... 
            var groupChat = groupRepositoty.groupChats.Where(g => g.groupId == groupId).
                            Select(x =>new {groupChatId=x.groupChatId, uid=x.users.userId,name=x.users.userName ,msg=x.chatTxt,group=x.groups.groupName,time=x.txtDate, txtType=x.txtType,txtStatus=x.txtStatus })
                            .OrderBy(g=>g.time)
                            .Skip(skiptotal).Take(pageLength);

           
                

        Clients.Caller.loadMessages(groupChat);
        }
        public void retriveUserChat(User selectedUser,User prmUser)
        {
            // we are going to check if selected user is alive(Online) or Not..

            // This Linq Query Retrive the communication between two selectedUsers...
            List<int> userss = new List<int> { selectedUser.userId, prmUser.userId };
            var userChat = from x in userRepository.chats where userss.Contains(x.senderId) && userss.Contains(x.receiverId)
                            select(new { uid = x.sender.userId, name = x.sender.userName, msg = x.commentTxt, time = x.commentDate });
            
            // Now we are retriving the chat of two users ,Now we have to set the status of Message to Read mean 1...
            // Call the Repository and set the message status to 1 Read or 1 ...

            Clients.Caller.loadUserChat(userChat);

        }
        public async Task sendMessageTo(string message, User senderUser,User receiverUser)
        {
            message tmessage = new message();
            string connectionId ;
            userRepository.userMessage(new userChat
            {
                commentTxt=message,
                commentDate=DateTime.Now,
                senderId=senderUser.userId,
                receiverId=receiverUser.userId,
                messageStatus=false
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
            if (connectionId!=null && connectionId!="") // check if user is onlne then transfer message oterwise message is saved offline for view
            {
              await  Clients.Client(connectionId).displayUserMessage(tmessage);

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

            var oflineUser = groupRepositoty.groupUsers.Where(g => g.groupId == 1 && g.users.userId == userId).Select(d => new { userId = d.users.userId, name = d.users.userName, group = d.groupId, status = d.users.connectionId }).FirstOrDefault();

            Clients.All.setOfline(oflineUser);
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
       
    }
}