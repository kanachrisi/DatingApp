using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Member> Users { get; set; }

        public DbSet<MemberLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
        }
    }
}
