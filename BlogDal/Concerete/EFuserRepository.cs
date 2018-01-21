using BlogDal.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class EFuserRepository : IuserRepository
    {
        private EFDbcontext context = new EFDbcontext();


       

        IQueryable<userChat> IuserRepository.chats
        {
            get
            {
                return context.userChat;
            }
        }

        IQueryable<user> IuserRepository.users
        {
            get
            {
                return context.users;
            }
        }
        public bool deleteUser(int userId)
        {
            user dbUser = context.users.Where(c => c.userId == userId).FirstOrDefault();
            if (dbUser!=null)
            {
                context.users.Remove(dbUser);
                context.SaveChanges();
                return true;

            }
            else
            {
                return false;
            }
            
        }

        public void disConnection(string connectionId)
        {
            user dbuser = context.users.Where(c => c.connectionId == connectionId).FirstOrDefault();
            if (dbuser != null)
            {
                dbuser.connectionId = null;
            }
            context.SaveChanges();

        }

        public void saveConnection(int userId, string conectionId)
        {
            user dbuser = context.users.Where(c => c.userId == userId).FirstOrDefault();
            if (dbuser!=null)
            {
                dbuser.connectionId = conectionId;
            }
            context.SaveChanges();
        }
        public void changeUserStatus(int userId,string value)
        {
            user dbUser = context.users.Where(c => c.userId == userId).FirstOrDefault();
            if (dbUser!=null)
            {
                dbUser.status = value;
            }
            context.SaveChanges();
        }
        public void saveGeoLocation(int userId, decimal lat, decimal lng)
        {
            user dbuser = context.users.Where(c => c.userId == userId).FirstOrDefault();
            if (dbuser != null)
            {
                dbuser.lat = lat;
                dbuser.lng = lng;
            }
            context.SaveChanges();
        }

        public string getConnction(int userId)
        {
            user dbuser = context.users.Where(c => c.userId == userId).FirstOrDefault();
            if (dbuser != null)
            {
                return dbuser.connectionId;
            }
            else
            return null;
        }

        public int getUserId(string connectionId)
        {
            user dbuser = context.users.Where(c => c.connectionId == connectionId).FirstOrDefault();
            if (dbuser != null)
            {
                return dbuser.userId;

            }
            else
                return 0;
        }

        public void userMessage (userChat prmChat)
        {
            context.userChat.Add(prmChat);
            context.SaveChanges();
        }
        public void Create(user prmUser)
        {
            context.users.Add(prmUser);
            context.SaveChanges();
        }
        public void saveCountry(int userId, string country)
        {
            user dbuser = context.users.Where(c => c.userId == userId).FirstOrDefault();
            if (dbuser != null)
            {
                dbuser.country = "http://www.geonames.org/flags/m/"+ country.ToLower() +".png";
            }
            context.SaveChanges();

        }

        public void Edit(user prmUer)
        {
            throw new NotImplementedException();
        }

        public user verifyUser(user prmUser)
        {
           user dbUser= context.users.Where(u => u.userName == prmUser.userName && u.password == prmUser.password).FirstOrDefault();
            return dbUser;
        }

        public bool checkUser(user prmUser)
        {
            user dbUser = context.users.Where(u => u.userName == prmUser.userName).FirstOrDefault();
            if (dbUser != null) return true;
            else return false;
        }
        public void changeMessageStatus(int userId)
        {
            
        }
    }
}
