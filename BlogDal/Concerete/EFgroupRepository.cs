using BlogDal.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class EFgroupRepository : IgroupRepository
    {
        private EFDbcontext context = new EFDbcontext();
        public IQueryable<group> groups
        {
            get
            {
                return context.groups;
            }
        }
        public IQueryable<groupChat> groupChats
        {
            get
            {
                return context.groupChats;
            }
        }


        public IQueryable<groupUser> groupUsers
        {
            get
            {
                return context.groupUsers;
            }
        }

        public void addGroup(string groupName)
        {
        }

        public void joinGroup(groupUser prmUser)
        {
            context.groupUsers.Add(prmUser);
            context.SaveChanges();
        }

        public void removeUser(groupUser prmUser)
        {

        }
        public string  getName(int groupId)
        {
            return context.groups.Where(g => g.groupId == groupId).Select(x => x.groupName).FirstOrDefault().ToString();
        }
        public void addMessage(groupChat prmMessage)
        {
              context.groupChats.Add(prmMessage);
              context.SaveChanges();
        }
        public void markAsDelete(int Id)
        {
            groupChat dbchat = context.groupChats.Where(c => c.groupChatId == Id).FirstOrDefault();
            dbchat.txtStatus = "delete";
            context.SaveChanges();
        }
        public string  markAsUndo(int Id)
        {
            groupChat dbchat = context.groupChats.Where(c => c.groupChatId == Id).FirstOrDefault();
            dbchat.txtStatus = null;
            context.SaveChanges();
            return dbchat.chatTxt;
        }

        public IList<string> getFileNames(int Id)
        {
            
                     
                     
                     
          return context.groupChats.Where(g => g.groupId == Id && g.txtType=="imagetxt").Select(x => x.chatTxt).ToList();
        }
        public int GroupMessageTotal(int groupId)
        {
            return context.groupChats.Where(c => c.groupId == groupId).ToList().Count();
        }

    }
}
