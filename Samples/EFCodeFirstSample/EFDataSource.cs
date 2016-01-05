using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;

namespace EFCodeFirstSample
{
    public class UserProfile
    {
        public int UserProfileId { set; get; }
        public string UserName { set; get; }

        [ForeignKey("CartableId")]
        public virtual Cartable Cartable { set; get; } // one-to-zero-or-one
        public int? CartableId { set; get; }

        public virtual ICollection<Doc> Docs { set; get; } // one-to-many
    }

    public class Doc
    {
        public int DocId { set; get; }
        public string Title { set; get; }
        public string Body { set; get; }

        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { set; get; }
        public int? UserProfileId { set; get; }

        public virtual ICollection<Cartable> Cartables { set; get; } // many-to-many        
    }

    public class Cartable
    {
        public int CartableId { set; get; }

        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { set; get; }
        public int? UserProfileId { set; get; }

        public virtual ICollection<Doc> Docs { set; get; } // many-to-many

        public Cartable()
        {
            Docs = new List<Doc>();
        }
    }

    public class UserProfileMap : EntityTypeConfiguration<UserProfile>
    {
        public UserProfileMap()
        {
            this.HasOptional(x => x.Cartable)
                .WithRequired(x => x.UserProfile)
                .WillCascadeOnDelete();
        }
    }

    public class MyContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Doc> Docs { get; set; }
        public DbSet<Cartable> Cartables { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserProfileMap());
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Configuration : DbMigrationsConfiguration<MyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(MyContext context)
        {
            //if (context.Database.Exists())
                //return;

            var user1 = new UserProfile { UserName = "Vahid" };
            context.UserProfiles.Add(user1);
            context.SaveChanges();

            var cartable1 = new Cartable { UserProfile = user1 };
            context.Cartables.Add(cartable1);
            context.SaveChanges();

            user1.CartableId = cartable1.CartableId;
            cartable1.UserProfileId = user1.UserProfileId;
            context.SaveChanges();

            var doc1 = new Doc { Title = "Title....1", Body = "Body....1", UserProfileId = user1.UserProfileId };
            var doc2 = new Doc { Title = "Title....2", Body = "Body....2", UserProfileId = user1.UserProfileId };
            context.Docs.Add(doc1);
            context.Docs.Add(doc2);
            context.SaveChanges();

            cartable1.Docs.Add(doc1);
            cartable1.Docs.Add(doc2);

            base.Seed(context);
        }
    }
}