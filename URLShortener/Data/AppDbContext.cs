using Microsoft.EntityFrameworkCore;
using URLShortener.Models;

namespace URLShortener.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ShortUrl> ShortUrls { get; set; }
        public DbSet<AboutContent> AboutContents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Username)
                    .IsUnique();

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Role)
                    .HasConversion<int>()
                    .IsRequired();
            });

            modelBuilder.Entity<ShortUrl>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.OriginalUrl)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(x => x.ShortCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(x => x.ShortCode)
                    .IsUnique();

                entity.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()")
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(u => u.ShortUrls)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AboutContent>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Content)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");
            });
        }
    }
}
