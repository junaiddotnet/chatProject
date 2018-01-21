using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Abstract
{
    public interface IgroupRepository
    {
        IQueryable<group> groups { get; }
        IQueryable<groupUser> groupUsers { get; }
        IQueryable<groupChat> groupChats { get; }

        void addGroup(string groupName);
        string getName(int groupId);
        void joinGroup(groupUser prmUser);
        void removeUser(groupUser prmUser);
        void addMessage(groupChat prmMessage);
        void markAsDelete(int Id);
        string markAsUndo(int Id);

        IList<string> getFileNames(int Id);
        int GroupMessageTotal(int groupId);
    }
}
