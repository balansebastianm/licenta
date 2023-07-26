using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LicWeb.Models;

namespace LicWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Doctor> Doctori { get; set; }
        public DbSet<Profesor> Profesori { get; set; }
        public DbSet<Student> Studenti { get; set; }
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public DbSet<User> Users { get; set; }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
        public DbSet<SituatieFinala> SituatiiFinale { get; set; }
        public DbSet<Prezenta> Prezente { get; set; }
        public DbSet<Materie> Materii { get; set; }
        public DbSet<Adeverinta> Adeverinte { get; set; }
        public DbSet<Cheie> Chei { get; set; }
        public DbSet<Orar> Oraruri { get; set; }
        public DbSet<Specializare> Specializari { get; set; }
        public DbSet<Modul> Moduluri { get; set; }

    }
}
