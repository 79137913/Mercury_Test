using Mercury_Test.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mercury_Test.DBContexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.Name)
                .HasColumnType("nvarchar(250)");

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.LastName)
                 .HasColumnType("nvarchar(250)");

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.Birthday)
                .HasColumnType("datetime2");

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.Gender)
                .HasColumnType("int");


        }
    }
}
