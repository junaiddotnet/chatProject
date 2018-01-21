using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class EFDbcontext:DbContext
    {
        public EFDbcontext():base("name=EFDbcontext")
        {

        }
        public DbSet<post> posts { get; set; }
        public DbSet<comment> comments{ get; set; }
        public DbSet<categorie> categories { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<group> groups { get; set; }
        public DbSet<groupChat> groupChats { get; set; }
        public DbSet<groupUser> groupUsers { get; set; }
        public DbSet<userChat> userChat { get; set; }
        public DbSet<contactus> contactuss { get; set; }
        public DbSet<project> projects { get; set; }
        public DbSet<favPost> favPosts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new postConfiguration());
            modelBuilder.Configurations.Add(new commentConfiguration());
            modelBuilder.Configurations.Add(new categorieConfiguration());
            modelBuilder.Configurations.Add(new userConfiguration());
            modelBuilder.Configurations.Add(new groupConfiguration());
            modelBuilder.Configurations.Add(new groupChatConfiguration());
            modelBuilder.Configurations.Add(new groupUserConfiguration());
            modelBuilder.Configurations.Add(new contactusConfiguration());
            modelBuilder.Configurations.Add(new projectConfiguration());



        }
    }
}
