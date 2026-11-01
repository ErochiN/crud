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
                ?? "Host=localhost;Port=5433;Database=appdb;Username=stepan;Password=A20!mY06@$e#sql";

            optionsBuilder.UseNpgsql(cs);
        }
    }
}
