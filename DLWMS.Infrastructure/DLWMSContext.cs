using DLWMS.Data;
using DLWMS.Data.IspitIB230306;
using Microsoft.EntityFrameworkCore;

using System.Configuration;

namespace DLWMS.Infrastructure
{
    public class DLWMSContext : DbContext
    {
      
        private string konekcijskiString = ConfigurationManager.ConnectionStrings["DLWMSBaza"].ConnectionString;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(konekcijskiString);
        }

        public DbSet<Student> Studenti { get; set; }
        public DbSet<StipendijeIB230306> StipendijeIB230306 { get; set; }
        public DbSet<StipendijeGodineIB230306> StipendijeGodineIB230306 { get; set; }
        public DbSet<StudentiStipendijeIB230306> StudentiStipendijeIB230306 { get; set; }

    }
}
