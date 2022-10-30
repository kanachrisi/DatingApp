using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<Member, AppRole, int, 
        IdentityUserClaim<int>, MemberRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

    
        public DbSet<MemberLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //..One Member can have Many Roles
            builder.Entity<Member>()
                .HasMany(m => m.MemberRoles)
                .WithOne(m => m.Member)
                .HasForeignKey(m => m.UserId)
                .IsRequired();

            //..One Role can belong to Many Members
            builder.Entity<AppRole>()
                .HasMany(m => m.MemberRoles)
                .WithOne(m => m.Role)
                .HasForeignKey(m => m.RoleId)
                .IsRequired();

            builder.Entity<MemberLike>()
                .HasKey(k => new { k.SourceMemberId, k.LikedMemberId });

            builder.Entity<MemberLike>()
                .HasOne(s => s.SourceMember)
                .WithMany(l => l.LikedMembers)
                .HasForeignKey(s => s.SourceMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MemberLike>()
                .HasOne(s => s.LikedMember)
                .WithMany(l => l.LikedByMembers)
                .HasForeignKey(s => s.LikedMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
