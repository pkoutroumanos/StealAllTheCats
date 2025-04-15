using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Models;

namespace StealAllTheCats.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CatEntity> Cats { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<CatTag> CatTags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatTag>()
                .HasKey(ct => new { ct.CatEntityId, ct.TagEntityId });

            modelBuilder.Entity<CatTag>()
                .HasOne(ct => ct.CatEntity)
                .WithMany(c => c.CatTags)
                .HasForeignKey(ct => ct.CatEntityId);

            modelBuilder.Entity<CatTag>()
                .HasOne(ct => ct.TagEntity)
                .WithMany(t => t.CatTags)
                .HasForeignKey(ct => ct.TagEntityId);
        }
    }
}
