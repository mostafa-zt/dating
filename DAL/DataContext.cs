using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DataContext : IdentityDbContext<AppUser, AppRole,
                                                int, IdentityUserClaim<int>,
                                                AppUserRole,
                                                IdentityUserLogin<int>,
                                                IdentityRoleClaim<int>,
                                                IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // var hashedPassword = HashPassowrd("123456");
            // modelBuilder.Entity<AppUser>().HasData(new AppUser() { Id = 1, Username = "John", PasswordHash = hashedPassword.FirstOrDefault().Value, PasswordSalt = hashedPassword.FirstOrDefault().Value },
            //                                        new AppUser() { Id = 2, Username = "Frank", PasswordHash = hashedPassword.FirstOrDefault().Value, PasswordSalt = hashedPassword.FirstOrDefault().Value },
            //                                        new AppUser() { Id = 3, Username = "Jayne", PasswordHash = hashedPassword.FirstOrDefault().Value, PasswordSalt = hashedPassword.FirstOrDefault().Value });
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().HasMany(x => x.UserRoles).WithOne(x => x.User).HasForeignKey(x => x.UserId).IsRequired();
            modelBuilder.Entity<AppRole>().HasMany(x => x.UserRoles).WithOne(x => x.Role).HasForeignKey(x => x.RoleId).IsRequired();

            modelBuilder.Entity<UserLike>().HasKey(x => new { x.SourceUserId, x.LikedUserId });
            modelBuilder.Entity<UserLike>().HasOne(s => s.SourceUser).WithMany(l => l.LikedUsers).HasForeignKey(x => x.SourceUserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserLike>().HasOne(s => s.LikedUser).WithMany(l => l.LikedByUsers).HasForeignKey(x => x.LikedUserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>().HasOne(s => s.Recipient).WithMany(l => l.MessagesReceived).HasForeignKey(x => x.RecipientId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().HasOne(s => s.Sender).WithMany(l => l.MessagesSent).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
        }

        // public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        // private Dictionary<byte[], byte[]> HashPassowrd(string password)
        // {
        //     var pass = new Dictionary<byte[], byte[]>();
        //     using var hmac = new HMACSHA512();
        //     pass.Add(hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        //     return pass;
        // }
    }
}