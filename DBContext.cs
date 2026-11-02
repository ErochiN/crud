using Microsoft.EntityFrameworkCore;

namespace CRDUProject
{
    public class DBContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Notes> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string cs = Environment.GetEnvironmentVariable("DB_CONNECTION")
                ?? "Host=localhost;Port=5433;Database=appdb;Username=stepan;Password=dev123";

            optionsBuilder.UseNpgsql(cs);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Notes>().HasIndex(n => new { n.User_Id, n.Name }).IsUnique();
        }
    }
}
