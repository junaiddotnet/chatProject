using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Abstract
{
    public interface IuserRepository
    {
        IQueryable<userChat> chats { get; }
        IQueryable<user> users { get; }

        void userMessage(userChat prmChat);
        string getConnction(int userId);
        int getUserId(string connectionId);
        void saveConnection(int userId,string conectionId);
        void saveGeoLocation(int userId, decimal lat, decimal lng);
        void disConnection(string connectionId);
        void Create(user prmUser);
        void Edit(user prmUer);
         user verifyUser(user prmUser);
        bool checkUser(user prmUser);
        bool deleteUser(int userId);
        void saveCountry(int userId, string country);
        void changeUserStatus(int userId, string value);

    }
}
