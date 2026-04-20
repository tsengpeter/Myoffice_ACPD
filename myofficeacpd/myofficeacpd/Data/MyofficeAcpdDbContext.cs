using Microsoft.EntityFrameworkCore;
using myofficeacpd.Data.Entities;

namespace myofficeacpd.Data
{
    public class MyofficeAcpdDbContext : DbContext
    {
        public MyofficeAcpdDbContext(DbContextOptions<MyofficeAcpdDbContext> options) : base(options) { }

        public DbSet<MyOfficeAcpd> MyOfficeAcpds { get; set; }
        public DbSet<MyOfficeExcuteionLog> MyOfficeExcuteionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyOfficeAcpd>(entity =>
            {
                entity.Property(e => e.AcpdStatus).HasDefaultValue((byte)0);
                entity.Property(e => e.AcpdStop).HasDefaultValue(false);
                entity.Property(e => e.AcpdNowDateTime).HasDefaultValueSql("getdate()");
                entity.Property(e => e.AcpdUpdDateTime).HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MyOfficeExcuteionLog>(entity =>
            {
                entity.Property(e => e.DeLogIsCustomDebug).HasDefaultValue(false);
                entity.Property(e => e.DeLogVerifyNeeded).HasDefaultValue(false);
                entity.Property(e => e.DeLogExDateTime).HasDefaultValueSql("getdate()");
            });
        }
    }
}
