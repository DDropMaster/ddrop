﻿using System.Data.Entity;
using DDrop.Db.DbEntities;

namespace DDrop.Db
{
    public class DDropContext : DbContext
    {
        public DDropContext() : base("DDropDataBase")
        {
        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbPlot> Plots { get; set; }
        public DbSet<DbSeries> Series { get; set; }
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
                .HasForeignKey(s => s.CurrentUserId);

            modelBuilder.Entity<DbUser>()
                .HasMany(s => s.Plots)
                .WithRequired(g => g.CurrentUser)
                .HasForeignKey(s => s.CurrentUserId);

            modelBuilder.Entity<DbSeries>()
                .HasMany(s => s.MeasurementsSeries)
                .WithRequired(g => g.CurrentSeries)
                .HasForeignKey(s => s.CurrentSeriesId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbSeries>()
                .HasRequired(s => s.ReferencePhotoForSeries)
                .WithRequiredPrincipal(ad => ad.Series);

            modelBuilder.Entity<DbSeries>()
                .HasRequired(s => s.Substance)
                .WithRequiredPrincipal(ad => ad.Series);

            modelBuilder.Entity<DbMeasurement>()
                .HasRequired(s => s.Drop)
                .WithRequiredPrincipal(ad => ad.Measurement);

            modelBuilder.Entity<DbMeasurement>()
                .HasOptional(c => c.FrontDropPhoto)
                .WithMany()
                .HasForeignKey(s => s.FrontDropPhotoId);

            modelBuilder.Entity<DbMeasurement>()
                .HasOptional(c => c.SideDropPhoto)
                .WithMany()
                .HasForeignKey(s => s.SideDropPhotoId);

            modelBuilder.Entity<DbDropPhoto>()
                .HasOptional(c => c.Contour)
                .WithMany()
                .HasForeignKey(s => s.ContourId);

            modelBuilder.Entity<DbThermalPhoto>()
                .HasOptional(c => c.Contour)
                .WithMany()
                .HasForeignKey(s => s.ContourId);

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