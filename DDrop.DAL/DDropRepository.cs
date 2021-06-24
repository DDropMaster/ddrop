using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DDrop.Db;
using DDrop.Db.DbEntities;

namespace DDrop.DAL
{
    public class DDropRepository : IDDropRepository
    {
        #region Logger

        public async Task SaveLogEntry(DbLogEntry dbLogEntry)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.LogEntries.Add(dbLogEntry);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region User

        public async Task CreateUserAsync(DbUser user)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Users.Add(user);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateUserAsync(DbUser user)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var userToUpdate = await context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);

                    context.Entry(userToUpdate).CurrentValues.SetValues(user);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<DbUser> GetUserByLogin(string email)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var users = await context.Users.Include(x => x.Plots).FirstOrDefaultAsync(x => x.Email == email);

                    if (users != null)
                    {
                        users.Plots = users.Plots.Where(x => x.Series == null).ToList();
                        users.UserSeries = null;
                    }

                    return users;
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task CreatePlot(DbPlot plot)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Users.Attach(plot.CurrentUser);

                    if (plot.Series != null)
                    {
                        context.Series.Attach(plot.Series);
                    }

                    context.Plots.Add(plot);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeletePlot(DbPlot plot)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var savedPlot = context.Plots.FirstOrDefault(x => x.PlotId == plot.PlotId);

                    context.Plots.Remove(savedPlot ?? throw new InvalidOperationException());

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdatePlotName(string text, Guid plotId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Plots.FirstOrDefaultAsync(x => x.PlotId == plotId);

                    if (series != null) series.Name = text;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdatePlot(DbPlot plotToUpdate)
        {
            using (var context = new DDropContext())
            {
                var plot = await context.Plots.FirstOrDefaultAsync(x => x.PlotId == plotToUpdate.PlotId);

                if (plot != null)
                {
                    plot.Points = plotToUpdate.Points;
                    plot.Settings = plotToUpdate.Settings;

                    await context.SaveChangesAsync();
                }
            }
        }

        #endregion

        #region Series

        public async Task CreateSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Series.Add(series);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateRegionOfInterest(Guid seriesId, string regionOfInterest)
        {
            using (var context = new DDropContext())
            {
                var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == seriesId);

                if (series != null)
                {
                    series.RegionOfInterest = regionOfInterest;

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateSeriesSettings(Guid seriesId, string settings)
        {
            using (var context = new DDropContext())
            {
                var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == seriesId);

                if (series != null)
                {
                    series.Settings = settings;

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteDrop(DbDrop drop)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Drops.Attach(drop);

                    var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.Drop.DropId == drop.DropId);

                    if (measurement != null)
                    {
                        measurement.Drop = null;
                    }

                    context.Drops.Remove(drop);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteComment(DbComment comment)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    if (comment.Type == "DropPhoto")
                    {
                        var dropPhoto = await context.DropPhotos.FirstOrDefaultAsync(x => x.CommentId == comment.CommentId);

                        if (dropPhoto != null)
                        {
                            dropPhoto.CommentId = null;
                        }
                    }
                    else if (comment.Type == "ThermalPhoto")
                    {
                        var thermalPhoto = await context.ThermalPhotos.FirstOrDefaultAsync(x => x.CommentId == comment.CommentId);

                        if (thermalPhoto != null)
                        {
                            thermalPhoto.CommentId = null;
                        }
                    }
                    else if (comment.Type == "Series")
                    {
                        var series = await context.Series.FirstOrDefaultAsync(x => x.CommentId == comment.CommentId);

                        if (series != null)
                        {
                            series.CommentId = null;
                        }
                    }
                    else if (comment.Type == "Measurement")
                    {
                        var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.CommentId == comment.CommentId);

                        if (measurement != null)
                        {
                            measurement.CommentId = null;
                        }
                    }

                    context.Comments.Attach(comment);
                    context.Comments.Remove(comment);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateComment(DbComment comment, Guid entityId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    if (comment.Type == "DropPhoto")
                    {
                        var dropPhoto = await context.DropPhotos.FirstOrDefaultAsync(x => x.PhotoId == entityId);

                        if (dropPhoto != null) dropPhoto.CommentId = comment.CommentId;
                    }
                    else if (comment.Type == "ThermalPhoto")
                    {
                        var thermalPhoto = await context.ThermalPhotos.FirstOrDefaultAsync(x => x.PhotoId == entityId);

                        if (thermalPhoto != null) thermalPhoto.CommentId = comment.CommentId;
                    }
                    else if (comment.Type == "Series")
                    {
                        var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == entityId);

                        if (series != null) series.CommentId = comment.CommentId;
                    }
                    else if (comment.Type == "Measurement")
                    {
                        var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.MeasurementId == entityId);

                        if (measurement != null) measurement.CommentId = comment.CommentId;
                    }

                    context.Set<DbComment>().AddOrUpdate(comment);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<List<DbSeries>> GetSeriesByUserId(Guid dbUserId)
        {
            using (var context = new DDropContext())
            {
                context.Database.CommandTimeout = 999999999;
                context.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                try
                {
                    var loadedSeries = await context.Series
                        .AsNoTracking()
                        .Where(x => x.CurrentUserId == dbUserId)
                        .Select(x => new
                        {
                            x.Title,
                            x.SeriesId,
                            x.IntervalBetweenPhotos,
                            x.AddedDate,
                            x.CurrentUserId,
                            x.CommentId,
                            x.Comment,
                            x.RegionOfInterest,
                            x.Settings,
                            x.Substance,
                            MeasurementsSeries = x.MeasurementsSeries.OrderBy(ms => ms.MeasurementOrderInSeries).Select(s => new
                            {
                                s.Name,
                                s.CurrentSeriesId,
                                s.MeasurementId,
                                s.AddedDate,
                                s.CreationDateTime,
                                s.MeasurementOrderInSeries,
                                s.AmbientTemperature,
                                DropPhotos = s.DropPhotos.OrderBy(dp => dp.PhotoType).Select(p => new
                                {
                                    p.Name,
                                    p.VerticalLine,
                                    p.HorizontalLine,
                                    p.PhotoId,
                                    p.AddedDate,
                                    p.PhotoType,
                                    p.CreationDateTime,
                                    p.XDiameterInPixels,
                                    p.YDiameterInPixels,
                                    p.ZDiameterInPixels,
                                    p.CommentId,
                                    p.ContourId,
                                    p.Comment,
                                }),
                                Drop = new
                                {
                                   s.Drop.DropId,
                                   s.Drop.XDiameterInMeters,
                                   s.Drop.YDiameterInMeters,
                                   s.Drop.ZDiameterInMeters,
                                   s.Drop.VolumeInCubicalMeters,
                                   s.Drop.RadiusInMeters,
                                   s.Drop.Temperature,
                                },
                                s.CommentId,
                                s.Comment,
                                ThermalPhoto = s.ThermalPhoto != null ? new
                                {
                                    s.ThermalPhoto.Name,
                                    s.ThermalPhoto.PhotoId,
                                    s.ThermalPhoto.AddedDate,
                                    s.ThermalPhoto.PhotoType,
                                    s.ThermalPhoto.CreationDateTime,
                                    s.ThermalPhoto.EllipseCoordinate,
                                    s.ThermalPhoto.CommentId,
                                    s.ThermalPhoto.Comment
                                } : null,
                            }),
                            ReferencePhotoForSeries = x.ReferencePhotoForSeries.Select(z => new
                            {
                                z.Name,
                                z.PixelsInMillimeter,
                                z.ReferenceLine,
                                z.PhotoId,
                                z.AddedDate,
                                z.CreationDateTime,
                                z.PhotoType,
                                z.CurrentSeriesId,
                            })
                        }).ToArrayAsync();

                    var dbSeries = new List<DbSeries>();

                    foreach (var series in loadedSeries)
                    {
                        var seriesToAdd = new DbSeries
                        {
                            Title = series.Title,
                            AddedDate = series.AddedDate,
                            CurrentUserId = series.CurrentUserId,
                            IntervalBetweenPhotos = series.IntervalBetweenPhotos,
                            SeriesId = series.SeriesId,
                            CommentId = series.CommentId,
                            RegionOfInterest = series.RegionOfInterest,
                            Settings = series.Settings,
                            Substance = series.Substance,
                            Comment = series.Comment,
                            MeasurementsSeries = series.MeasurementsSeries.Select(m => new DbMeasurement
                            {
                                Name = m.Name,
                                CurrentSeriesId = m.CurrentSeriesId,
                                MeasurementId = m.MeasurementId,
                                AddedDate = m.AddedDate,
                                CreationDateTime = m.CreationDateTime,
                                MeasurementOrderInSeries = m.MeasurementOrderInSeries,
                                AmbientTemperature = m.AmbientTemperature,
                                Drop = new DbDrop
                                { 
                                    DropId = m.Drop.DropId,
                                    RadiusInMeters = m.Drop.RadiusInMeters,
                                    Temperature = m.Drop.Temperature,
                                    VolumeInCubicalMeters = m.Drop.VolumeInCubicalMeters,
                                    XDiameterInMeters = m.Drop.XDiameterInMeters,
                                    YDiameterInMeters = m.Drop.YDiameterInMeters,
                                    ZDiameterInMeters = m.Drop.ZDiameterInMeters
                                },
                                CommentId = m.CommentId,
                                Comment = m.Comment,
                                ThermalPhoto = m.ThermalPhoto != null ? new DbThermalPhoto
                                {
                                    Name = m.ThermalPhoto.Name,
                                    PhotoId = m.ThermalPhoto.PhotoId,
                                    AddedDate = m.ThermalPhoto.AddedDate,
                                    PhotoType = m.ThermalPhoto.PhotoType,
                                    CreationDateTime = m.ThermalPhoto.CreationDateTime,
                                    EllipseCoordinate = m.ThermalPhoto.EllipseCoordinate,
                                    CommentId = m.ThermalPhoto.CommentId
                                } : null,
                                DropPhotos = m.DropPhotos.Select(p => new DbDropPhoto
                                {
                                    Name = p.Name,
                                    VerticalLine = p.VerticalLine,
                                    HorizontalLine = p.HorizontalLine,
                                    PhotoId = p.PhotoId,
                                    AddedDate = p.AddedDate,
                                    PhotoType = p.PhotoType,
                                    CreationDateTime = p.CreationDateTime,
                                    XDiameterInPixels = p.XDiameterInPixels,
                                    YDiameterInPixels = p.YDiameterInPixels,
                                    ZDiameterInPixels = p.ZDiameterInPixels,
                                    CommentId = p.CommentId,
                                    ContourId = p.ContourId,
                                    Comment = p.Comment,
                                }).ToList(),
                            }).ToList(),
                            ReferencePhotoForSeries = series.ReferencePhotoForSeries.Select(z => new DbReferencePhoto
                            {
                                Name = z.Name,
                                PixelsInMillimeter = z.PixelsInMillimeter,
                                ReferenceLine = z.ReferenceLine,
                                PhotoId = z.PhotoId,
                                AddedDate = z.AddedDate,
                                CreationDateTime = z.CreationDateTime,
                                PhotoType = z.PhotoType,
                                CurrentSeriesId = z.CurrentSeriesId,
                            }).ToList()
                        };

                        seriesToAdd.ThermalPlot = await GetSeriesPlot(seriesToAdd.SeriesId, seriesToAdd);

                        dbSeries.Add(seriesToAdd);
                    }

                    return dbSeries;
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        private async Task<DbPlot> GetSeriesPlot(Guid seriesId, DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var seriesPlot = await context.Plots
                    .Where(x => x.Series.SeriesId == seriesId)
                    .Select(x => new
                    {
                        x.Name,
                        x.Points,
                        x.CurrentUserId,
                        x.PlotId,
                        x.PlotType,
                        x.Settings,
                    }).FirstOrDefaultAsync();

                if (seriesPlot == null)
                {
                    return null;
                }

                return new DbPlot
                {
                    Name = seriesPlot.Name,
                    Series = series,
                    Points = seriesPlot.Points,
                    CurrentUserId = seriesPlot.CurrentUserId,
                    PlotType = seriesPlot.PlotType,
                    PlotId = seriesPlot.PlotId,
                    Settings = seriesPlot.Settings
                };
            }
        }

        public async Task<List<DbMeasurement>> GetMeasurements(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var measurementForSeries = await context.Measurements.Where(x => x.CurrentSeriesId == series.SeriesId)
                    .Select(x => new
                    {
                        x.Name,
                        x.CurrentSeries,
                        x.CurrentSeriesId,
                        x.MeasurementId,
                        x.AddedDate,
                        x.CreationDateTime,
                        x.MeasurementOrderInSeries,
                        x.AmbientTemperature,
                        DropPhotos = x.DropPhotos.OrderBy(dp => dp.PhotoType).Select(p => new
                        {
                            p.Name,
                            p.VerticalLine,
                            p.HorizontalLine,
                            p.PhotoId,
                            p.AddedDate,
                            p.PhotoType,
                            p.CreationDateTime,
                            p.XDiameterInPixels,
                            p.YDiameterInPixels,
                            p.ZDiameterInPixels,
                            p.CommentId,
                            p.ContourId,
                            p.Comment,
                        }),
                        x.Drop,
                        x.CommentId
                    })
                    .OrderBy(x => x.MeasurementOrderInSeries)
                    .ToListAsync();

                var dbMeasurementsForAdd = new List<DbMeasurement>();

                foreach (var measurement in measurementForSeries)
                {
                    var measurementToAdd = new DbMeasurement
                    {
                        AddedDate = measurement.AddedDate,
                        CurrentSeries = series,
                        DropPhotos = measurement.DropPhotos.Select(p => new DbDropPhoto
                        {
                            Name = p.Name,
                            VerticalLine = p.VerticalLine,
                            HorizontalLine = p.HorizontalLine,
                            PhotoId = p.PhotoId,
                            AddedDate = p.AddedDate,
                            PhotoType = p.PhotoType,
                            CreationDateTime = p.CreationDateTime,
                            XDiameterInPixels = p.XDiameterInPixels,
                            YDiameterInPixels = p.YDiameterInPixels,
                            ZDiameterInPixels = p.ZDiameterInPixels,
                            CommentId = p.CommentId,
                            ContourId = p.ContourId,
                            Comment = p.Comment,
                        }).ToList(),
                        CurrentSeriesId = measurement.CurrentSeriesId,
                        MeasurementId = measurement.MeasurementId,
                        Name = measurement.Name,
                        CreationDateTime = measurement.CreationDateTime,
                        MeasurementOrderInSeries = measurement.MeasurementOrderInSeries,
                        AmbientTemperature = measurement.AmbientTemperature,
                        CommentId = measurement.CommentId
                    };

                    measurementToAdd.ThermalPhoto = await GetDbThermalPhoto(measurementToAdd);
                    if (measurementToAdd.CommentId != null)
                        measurementToAdd.Comment = await GetCommentById(measurementToAdd.CommentId.Value);

                    var dropToAdd = await context.Drops.FirstOrDefaultAsync(x => x.DropId == measurement.Drop.DropId);

                    if (dropToAdd != null)
                    {
                        measurementToAdd.Drop = new DbDrop
                        {
                            Measurement = measurementToAdd,
                            DropId = dropToAdd.DropId,
                            RadiusInMeters = dropToAdd.RadiusInMeters,
                            Temperature = dropToAdd.Temperature,
                            VolumeInCubicalMeters = dropToAdd.VolumeInCubicalMeters,
                            XDiameterInMeters = dropToAdd.XDiameterInMeters,
                            YDiameterInMeters = dropToAdd.YDiameterInMeters,
                            ZDiameterInMeters = dropToAdd.ZDiameterInMeters
                        };
                    }

                    dbMeasurementsForAdd.Add(measurementToAdd);
                }

                return dbMeasurementsForAdd;
            }
        }

        public async Task<DbMeasurement> GetMeasurement(Guid measurementId)
        {
            using (var context = new DDropContext())
            {
                return await context.Measurements.FirstOrDefaultAsync(x => x.MeasurementId == measurementId);
            }
        }

        public async Task<List<DbDropPhoto>> GetDropPhotosByMeasurementId(Guid measurementId)
        {
            using (var context = new DDropContext())
            {
                var dropPhotos = await context.DropPhotos
                    .Where(x => x.MeasurementId == measurementId)
                    .Select(p => new
                    {
                        Name = p.Name,
                        VerticalLine = p.VerticalLine,
                        HorizontalLine = p.HorizontalLine,
                        PhotoId = p.PhotoId,
                        AddedDate = p.AddedDate,
                        PhotoType = p.PhotoType,
                        CreationDateTime = p.CreationDateTime,
                        XDiameterInPixels = p.XDiameterInPixels,
                        YDiameterInPixels = p.YDiameterInPixels,
                        ZDiameterInPixels = p.ZDiameterInPixels,
                        CommentId = p.CommentId,
                        ContourId = p.ContourId,
                        Comment = p.Comment,
                    }).ToListAsync();

                var resultingDropPhotos = new List<DbDropPhoto>();

                foreach (var dbDropPhoto in dropPhotos)
                {
                    resultingDropPhotos.Add(new DbDropPhoto
                    {
                        Name = dbDropPhoto.Name,
                        VerticalLine = dbDropPhoto.VerticalLine,
                        HorizontalLine = dbDropPhoto.HorizontalLine,
                        PhotoId = dbDropPhoto.PhotoId,
                        AddedDate = dbDropPhoto.AddedDate,
                        PhotoType = dbDropPhoto.PhotoType,
                        CreationDateTime = dbDropPhoto.CreationDateTime,
                        XDiameterInPixels = dbDropPhoto.XDiameterInPixels,
                        YDiameterInPixels = dbDropPhoto.YDiameterInPixels,
                        ZDiameterInPixels = dbDropPhoto.ZDiameterInPixels,
                        CommentId = dbDropPhoto.CommentId,
                        ContourId = dbDropPhoto.ContourId,
                        Comment = dbDropPhoto.Comment,
                    });
                }

                return resultingDropPhotos;
            }
        }

        public async Task<List<DbReferencePhoto>> GetReferencePhotoById(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var referencePhotosForSeries = await context.ReferencePhotos
                    .Where(x => x.CurrentSeriesId == series.SeriesId)
                    .Select(x => new
                    {
                        x.Name,
                        x.PixelsInMillimeter,
                        x.ReferenceLine,
                        x.PhotoId,
                        x.Series,
                        x.AddedDate,
                        x.CreationDateTime,
                        x.PhotoType,
                        SeriesId = x.CurrentSeriesId
                    }).ToListAsync();

                var resultingReferencePhotos = new List<DbReferencePhoto>();

                foreach (var dbReferencePhoto in referencePhotosForSeries)
                {
                    resultingReferencePhotos.Add(new DbReferencePhoto
                    {
                        Name = dbReferencePhoto.Name,
                        PixelsInMillimeter = dbReferencePhoto.PixelsInMillimeter,
                        PhotoId = dbReferencePhoto.PhotoId,
                        ReferenceLine = dbReferencePhoto.ReferenceLine,
                        Series = series,
                        AddedDate = dbReferencePhoto.AddedDate,
                        CreationDateTime = dbReferencePhoto.CreationDateTime,
                        CurrentSeriesId = dbReferencePhoto.SeriesId,
                        PhotoType = dbReferencePhoto.PhotoType
                    });
                }

                return resultingReferencePhotos;
            }
        }

        private async Task<DbComment> GetCommentById(Guid commentId)
        {
            using (var context = new DDropContext())
            {
                var comment = await context.Comments
                    .Where(x => x.CommentId == commentId)
                    .Select(x => new
                    {
                        x.CommentId,
                        x.Content,
                        x.Type
                    }).FirstOrDefaultAsync();

                if (comment == null)
                {
                    return null;
                }

                return new DbComment()
                {
                    CommentId = comment.CommentId,
                    Content = comment.Content,
                    Type = comment.Type
                };
            }
        }

        private async Task<DbDropPhoto> GetDbDropPhotoById(Guid photoId)
        {
            using (var context = new DDropContext())
            {
                var dropPhoto = await context.DropPhotos
                    .Where(x => x.PhotoId == photoId)
                    .Select(x => new
                    {
                        x.Name,
                        x.Contour,
                        x.VerticalLine,
                        x.HorizontalLine,
                        x.PhotoId,
                        x.AddedDate,
                        x.PhotoType,
                        x.CreationDateTime,
                        x.XDiameterInPixels,
                        x.YDiameterInPixels,
                        x.ZDiameterInPixels,
                        x.CommentId,
                        x.ContourId
                    }).FirstOrDefaultAsync();

                if (dropPhoto == null)
                {
                    return null;
                }

                var dbDropPhoto = new DbDropPhoto
                {
                    VerticalLine = dropPhoto.VerticalLine,
                    HorizontalLine = dropPhoto.HorizontalLine,
                    Name = dropPhoto.Name,
                    XDiameterInPixels = dropPhoto.XDiameterInPixels,
                    AddedDate = dropPhoto.AddedDate,
                    CreationDateTime = dropPhoto.CreationDateTime,
                    PhotoId = dropPhoto.PhotoId,
                    PhotoType = dropPhoto.PhotoType,
                    YDiameterInPixels = dropPhoto.YDiameterInPixels,
                    ZDiameterInPixels = dropPhoto.ZDiameterInPixels,
                    CommentId = dropPhoto.CommentId,
                    ContourId = dropPhoto.ContourId
                };

                if (dbDropPhoto.CommentId != null)
                {
                    dbDropPhoto.Comment = await GetCommentById(dbDropPhoto.CommentId.Value);
                }

                if (dbDropPhoto.ContourId != null)
                {
                    dbDropPhoto.Contour = await GetContourById(dbDropPhoto.ContourId.Value);
                }

                return dbDropPhoto;
            }
        }

        private async Task<DbThermalPhoto> GetDbThermalPhoto(DbMeasurement measurement)
        {
            using (var context = new DDropContext())
            {
                var thermalPhoto = context.ThermalPhotos
                    .Where(x => x.PhotoId == measurement.MeasurementId)
                    .Select(x => new
                    {
                        x.Name,
                        x.PhotoId,
                        x.AddedDate,
                        x.PhotoType,
                        x.CreationDateTime,
                        x.EllipseCoordinate,
                        x.CommentId
                    }).FirstOrDefault();

                if (thermalPhoto == null)
                {
                    return null;
                }

                var dbThermalPhoto = new DbThermalPhoto()
                {
                    Name = thermalPhoto.Name,
                    AddedDate = thermalPhoto.AddedDate,
                    CreationDateTime = thermalPhoto.CreationDateTime,
                    PhotoId = thermalPhoto.PhotoId,
                    PhotoType = thermalPhoto.PhotoType,
                    Measurement = measurement,
                    EllipseCoordinate = thermalPhoto.EllipseCoordinate,
                    CommentId = thermalPhoto.CommentId
                };

                if (dbThermalPhoto.CommentId != null)
                    dbThermalPhoto.Comment = await GetCommentById(dbThermalPhoto.CommentId.Value);

                return dbThermalPhoto;
            }
        }

        private async Task<DbContour> GetContourById(Guid contourId)
        {
            using (var context = new DDropContext())
            {
                var contour = await context.Contours.FirstOrDefaultAsync(x => x.ContourId == contourId);

                if (contour == null)
                {
                    return null;
                }

                return new DbContour
                {
                    ConnectedLines = contour.ConnectedLines,
                    CalculationParameters = contour.CalculationParameters,
                    CalculationProvider = contour.CalculationProvider,
                    ContourId = contour.ContourId
                };
            }
        }

        public async Task<DbSeries> GetDbSeriesForExportById(Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Series.Where(x => x.SeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Title,
                            x.IntervalBetweenPhotos,
                        }).FirstOrDefaultAsync();

                    var referencePhotoForSeries = context.ReferencePhotos.Where(x => x.CurrentSeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Name,
                            x.PixelsInMillimeter,
                            x.ReferenceLine,
                            x.CreationDateTime,
                            x.AddedDate,
                            x.PhotoType,
                            x.Content,
                        }).ToList();

                    var measurementForSeries = context.Measurements.Where(x => x.CurrentSeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Name,
                            x.CreationDateTime,
                            x.MeasurementOrderInSeries,
                            x.MeasurementId,
                            x.AddedDate,
                            x.AmbientTemperature,
                            DropPhotos = x.DropPhotos.OrderBy(dp => dp.PhotoType).Select(p => new
                            {
                                p.Name,
                                p.VerticalLine,
                                p.HorizontalLine,
                                p.PhotoId,
                                p.AddedDate,
                                p.PhotoType,
                                p.CreationDateTime,
                                p.XDiameterInPixels,
                                p.YDiameterInPixels,
                                p.ZDiameterInPixels,
                                p.CommentId,
                                p.ContourId,
                                p.Comment,
                            }),
                        }).ToList();

                    var dbMeasurementForAdd = new List<DbMeasurement>();

                    foreach (var measurement in measurementForSeries)
                    {
                        var measurementForAdd = new DbMeasurement
                        {
                            Name = measurement.Name,
                            CreationDateTime = measurement.CreationDateTime,
                            MeasurementOrderInSeries = measurement.MeasurementOrderInSeries,
                            AmbientTemperature = measurement.AmbientTemperature,
                            DropPhotos = measurement.DropPhotos.Select(p => new DbDropPhoto
                            {
                                Name = p.Name,
                                VerticalLine = p.VerticalLine,
                                HorizontalLine = p.HorizontalLine,
                                PhotoId = p.PhotoId,
                                AddedDate = p.AddedDate,
                                PhotoType = p.PhotoType,
                                CreationDateTime = p.CreationDateTime,
                                XDiameterInPixels = p.XDiameterInPixels,
                                YDiameterInPixels = p.YDiameterInPixels,
                                ZDiameterInPixels = p.ZDiameterInPixels,
                                CommentId = p.CommentId,
                                ContourId = p.ContourId,
                                Comment = p.Comment,
                            }).ToList(),
                        };

                        var drop = await context.Drops.FirstOrDefaultAsync(x => x.Measurement.MeasurementId == measurement.MeasurementId);
                        if (drop != null) drop.DropId = Guid.Empty;

                        measurementForAdd.Drop = drop;

                        dbMeasurementForAdd.Add(measurementForAdd);
                    }

                    List<DbReferencePhoto> referencePhotoForAdd = new List<DbReferencePhoto>();

                    foreach (var photoForSeries in referencePhotoForSeries)
                    {
                        referencePhotoForAdd.Add(new DbReferencePhoto
                        {
                            Name = photoForSeries.Name,
                            PixelsInMillimeter = photoForSeries.PixelsInMillimeter,
                            ReferenceLine = photoForSeries.ReferenceLine,
                            AddedDate = photoForSeries.AddedDate,
                            Content = photoForSeries.Content,
                            CreationDateTime = photoForSeries.CreationDateTime,
                            PhotoType = photoForSeries.PhotoType
                        });
                    }
                    
                    return new DbSeries
                    {
                        Title = series?.Title,
                        IntervalBetweenPhotos = series.IntervalBetweenPhotos,
                        ReferencePhotoForSeries = referencePhotoForAdd,
                        MeasurementsSeries = dbMeasurementForAdd.OrderBy(x => x.MeasurementOrderInSeries).ToList()
                    };
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task CreateFullSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Users.Attach(series.CurrentUser);
                    context.Series.Add(series);

                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw new DbUpdateException(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        public async Task DeleteSingleSeries(DbSeries series)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    for (int i = series.MeasurementsSeries.Count - 1; i >= 0; i--)
                    {
                        await DeleteMeasurement(series.MeasurementsSeries[i]);
                    }

                    if (series.Substance != null)
                    {
                        await DeleteSubstance(series.Substance.SubstanceId);
                    }

                    if (series.Comment != null)
                    {
                        await DeleteComment(series.Comment);
                    }

                    context.Series.Attach(series);

                    context.Series.Remove(series);

                    bool saveFailed;

                    do
                    {
                        saveFailed = false;

                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            // Update the values of the entity that failed to save from the store
                            await ex.Entries.Single().ReloadAsync();
                        }

                    } while (saveFailed);

                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateSeriesName(string seriesName, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == seriesId);

                    if (series != null) series.Title = seriesName;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateSeriesIntervalBetweenPhotos(int interval, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var series = await context.Series.FirstOrDefaultAsync(x => x.SeriesId == seriesId);

                    if (series != null) series.IntervalBetweenPhotos = interval;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region Drop Photo

        public async Task CreateDropPhoto(DbDropPhoto dropPhoto, DbMeasurement measurement)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.DropPhotos.Add(dropPhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task CreateMeasurement(DbMeasurement measurement, Guid seriesId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Measurements.Add(measurement);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public void UpdateContour(DbContour contour)
        {
            using (var context = new DDropContext())
            {
                context.Set<DbContour>().AddOrUpdate(new DbContour
                {
                    ContourId = contour.ContourId,
                    CalculationParameters = contour.CalculationParameters,
                    CalculationProvider = contour.CalculationProvider,
                    ConnectedLines = contour.ConnectedLines
                });

                context.SaveChanges();
            }
        }

        public async Task DeleteContour(Guid contourId)
        {
            using (var context = new DDropContext())
            {
                var existingContour = await context.Contours.FirstOrDefaultAsync(x => x.ContourId == contourId);

                if (existingContour != null)
                {
                    var relatedPhoto = await context.DropPhotos.FirstOrDefaultAsync(x => x.ContourId == contourId);

                    if (relatedPhoto != null)
                    {
                        relatedPhoto.Contour = null;
                        relatedPhoto.ContourId = null;
                    }

                    var relatedThermalPhoto = await context.ThermalPhotos.FirstOrDefaultAsync(x => x.ContourId == contourId);

                    if (relatedThermalPhoto != null)
                    {
                        relatedThermalPhoto.Contour = null;
                        relatedThermalPhoto.ContourId = null;
                    }

                    context.Contours.Remove(existingContour);
                }

                context.SaveChanges();
            }
        }

        public async Task UpdateDropPhoto(DbDropPhoto dropPhoto, bool updateContent = false)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.DropPhotos.Attach(dropPhoto);

                    context.Entry(dropPhoto).State = EntityState.Modified;
                    
                    context.Entry(dropPhoto).Property(x => x.Content).IsModified = updateContent;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateDrop(DbDrop drop)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Set<DbDrop>().AddOrUpdate(drop);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateDropTemperature(double temperature, Guid dropId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var drop = await context.Drops.FirstOrDefaultAsync(x => x.DropId == dropId);

                    if (drop != null) drop.Temperature = temperature;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteDropPhoto(DbDropPhoto dropPhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.DropPhotos.Attach(dropPhoto);

                    context.DropPhotos.Remove(dropPhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateMeasurementName(string newName, Guid measurementId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.MeasurementId == measurementId);

                    if (measurement != null) measurement.Name = newName;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateDropPhotoName(string newName, Guid photoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var dropPhoto = await context.DropPhotos.FirstOrDefaultAsync(x => x.PhotoId == photoId);

                    if (dropPhoto != null) dropPhoto.Name = newName;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteMeasurement(DbMeasurement measurement)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    foreach (var dropPhoto in measurement.DropPhotos)
                    {
                        if (dropPhoto.Contour != null && dropPhoto.ContourId != null)
                        {
                            await DeleteContour(dropPhoto.ContourId.Value);
                        }

                        if (dropPhoto.Comment != null)
                        {
                            await DeleteComment(dropPhoto.Comment);
                        }

                        await DeleteDropPhoto(dropPhoto);
                    }

                    if (measurement.ThermalPhoto != null)
                    {
                        if (measurement.ThermalPhoto.Contour != null && measurement.ThermalPhoto.ContourId != null)
                        {
                            await DeleteContour(measurement.ThermalPhoto.ContourId.Value);
                        }

                        if (measurement.ThermalPhoto.Comment != null)
                        {
                            await DeleteComment(measurement.ThermalPhoto.Comment);
                        }

                        await DeleteThermalPhoto(measurement.ThermalPhoto);
                    }

                    if (measurement.Comment != null)
                    {
                        await DeleteComment(measurement.Comment);
                    }

                    context.Measurements.Attach(measurement);

                    context.Measurements.Remove(measurement);

                    await context.SaveChangesAsync();
                    
                }
                catch (DbUpdateException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<byte[]> GetPhotoContent(Guid photoId, CancellationToken cancellationToken)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var content = await context.DropPhotos.Where(x => x.PhotoId == photoId)
                        .Select(z => z.Content).FirstOrDefaultAsync().ConfigureAwait(true);
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    return content;
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateMeasurementsOrderInSeries(List<DbMeasurement> newDbMeasurements)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    foreach (var measurement in newDbMeasurements)
                    {
                        context.Measurements.Attach(measurement);
                        context.Entry(measurement).Property(x => x.MeasurementOrderInSeries).IsModified = true;
                    }

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateMeasurementAmbientTemperature(double temperature, Guid measurementId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.MeasurementId == measurementId);

                    if (measurement != null) measurement.AmbientTemperature = temperature;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region Substance

        public async Task UpdateSubstance(DbSubstances substance)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Set<DbSubstances>().AddOrUpdate(substance);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteSubstance(Guid substanceId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var substance =
                        await context.Substances.FirstOrDefaultAsync(x =>
                            x.SubstanceId == substanceId);

                    context.Substances.Attach(substance ?? throw new InvalidOperationException());

                    context.Substances.Remove(substance);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region Reference Photo

        public async Task UpdateReferencePhoto(DbReferencePhoto referencePhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.Set<DbReferencePhoto>().AddOrUpdate(referencePhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteReferencePhoto(Guid dbReferencePhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var referencePhoto =
                        await context.ReferencePhotos.FirstOrDefaultAsync(x =>
                            x.PhotoId == dbReferencePhotoId);

                    context.ReferencePhotos.Attach(referencePhoto ?? throw new InvalidOperationException());

                    context.ReferencePhotos.Remove(referencePhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<byte[]> GetReferencePhotoContent(Guid referencePhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return await context.ReferencePhotos.Where(x => x.PhotoId == referencePhotoId)
                        .Select(z => z.Content).FirstOrDefaultAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

        #region ThermalPhoto

        public async Task<byte[]> GetThermalPhotoContent(Guid photoId, CancellationToken cancellationToken)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var content = await context.ThermalPhotos.Where(x => x.PhotoId == photoId)
                        .Select(z => z.Content).FirstOrDefaultAsync().ConfigureAwait(true);

                    cancellationToken.ThrowIfCancellationRequested();

                    return content;
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateThermalPhoto(DbThermalPhoto dbPhoto, bool updateContent = false)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.ThermalPhotos.Attach(dbPhoto);

                    context.Entry(dbPhoto).State = EntityState.Modified;
                    context.Entry(dbPhoto).Property(x => x.Content).IsModified = updateContent;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task CreateThermalPhoto(DbThermalPhoto thermalPhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    context.ThermalPhotos.Add(thermalPhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task DeleteThermalPhoto(DbThermalPhoto thermalPhoto)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    thermalPhoto.Measurement = null;
                    context.ThermalPhotos.Attach(thermalPhoto);

                    var measurement = await context.Measurements.FirstOrDefaultAsync(x => x.MeasurementId == thermalPhoto.PhotoId);
                    
                    if (measurement != null)
                    {
                        measurement.ThermalPhoto = null;
                    }

                    context.ThermalPhotos.Remove(thermalPhoto);

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateThermalPhotoName(string text, Guid editedPhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var thermalPhoto = await context.ThermalPhotos.FirstOrDefaultAsync(x => x.PhotoId == editedPhotoId);

                    if (thermalPhoto != null) thermalPhoto.Name = text;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task UpdateThermalPhotoEllipseCoordinate(string temperatureCoordinate, Guid editedPhotoId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var thermalPhoto = await context.ThermalPhotos.FirstOrDefaultAsync(x => x.PhotoId == editedPhotoId);

                    if (thermalPhoto != null) thermalPhoto.EllipseCoordinate = temperatureCoordinate;

                    await context.SaveChangesAsync();
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        public async Task<DbContour> GetDbContour(Guid contourId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    return await context.Contours.FirstOrDefaultAsync(x => x.ContourId == contourId);
                }
                catch (SqlException e)
                {
                    throw new TimeoutException(e.Message, e);
                }
            }
        }

        #endregion

    }
}