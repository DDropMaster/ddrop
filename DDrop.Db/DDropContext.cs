using System.Data.Entity;
using DDrop.Db.DbEntities;

namespace DDrop.Db
{
    public class DDropContext : DbContext
    {
        public DDropContext() : base("DDropDataBase")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DDropContext>());
        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbPlot> Plots { get; set; }
        public DbSet<DbSeries> Series { get; set; }
        public DbSet<DbBasePhoto> BasePhotos { get; set; }
        public DbSet<DbMeasurement> Measurements { get; set; }
        public DbSet<DbDropPhoto> DropPhotos { get; set; }
        public DbSet<DbContour> Contours { get; set; }
        public DbSet<DbDrop> Drops { get; set; }
        public DbSet<DbReferencePhoto> ReferencePhotos { get; set; }
        public DbSet<DbThermalPhoto> ThermalPhotos { get; set; }
        public DbSet<DbLogEntry> LogEntries { get; set; }
        public DbSet<DbSubstances> Substances { get; set; }
        public DbSet<DbComment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>()
                .HasMany(s => s.UserSeries)
                .WithRequired(g => g.CurrentUser)
                .HasForeignKey(s => s.CurrentUserId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbUser>()
                .HasMany(s => s.Plots)
                .WithRequired(g => g.CurrentUser)
                .HasForeignKey(s => s.CurrentUserId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbPlot>()
                .HasOptional(x => x.Series)
                .WithOptionalDependent(s => s.ThermalPlot)
                .Map(x => x.MapKey("SeriesId"));

            modelBuilder.Entity<DbSeries>()
                .HasMany(s => s.MeasurementsSeries)
                .WithRequired(g => g.CurrentSeries)
                .HasForeignKey(s => s.CurrentSeriesId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbSeries>()
                .HasMany(s => s.ReferencePhotoForSeries)
                .WithRequired(g => g.Series)
                .HasForeignKey(s => s.CurrentSeriesId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbSeries>()
                .HasRequired(s => s.Substance)
                .WithRequiredPrincipal(ad => ad.Series)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbMeasurement>()
                .HasRequired(s => s.Drop)
                .WithRequiredPrincipal(ad => ad.Measurement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbMeasurement>()
                .HasMany(s => s.DropPhotos)
                .WithRequired(g => g.Measurement)
                .HasForeignKey(s => s.MeasurementId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbDropPhoto>()
                .HasOptional(c => c.Contour)
                .WithMany()
                .HasForeignKey(s => s.ContourId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbThermalPhoto>()
                .HasOptional(c => c.Contour)
                .WithMany()
                .HasForeignKey(s => s.ContourId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbMeasurement>()
                .HasOptional(c => c.Comment)
                .WithMany()
                .HasForeignKey(c => c.CommentId);

            modelBuilder.Entity<DbDropPhoto>()
                .HasOptional(c => c.Comment)
                .WithMany()
                .HasForeignKey(c => c.CommentId);

            modelBuilder.Entity<DbThermalPhoto>()
                .HasOptional(c => c.Comment)
                .WithMany()
                .HasForeignKey(c => c.CommentId);

            modelBuilder.Entity<DbSeries>()
                .HasOptional(c => c.Comment)
                .WithMany()
                .HasForeignKey(c => c.CommentId);

            modelBuilder.Entity<DbMeasurement>()
                .HasRequired(s => s.ThermalPhoto)
                .WithRequiredPrincipal(ad => ad.Measurement);
        }
    }
}