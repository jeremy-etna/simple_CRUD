

using Microsoft.EntityFrameworkCore;
using quest_web.Models;

namespace quest_web.DAL
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Address>().ToTable("address");
        }

        public DbSet<User> User { get; set; }

        public DbSet <Address> Address { get; set; }

    }
}