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
                    context.Users.Attach(series.CurrentUser);
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

        public List<DbSeries> GetSeriesByUserId(Guid dbUserId)
        {
            using (var context = new DDropContext())
            {
                try
                {
                    var loadedSeries = context.Series.Where(x => x.CurrentUserId == dbUserId)
                        .Select(x => new
                        {
                            x.Title,
                            x.SeriesId,
                            x.IntervalBetweenPhotos,
                            x.AddedDate,
                            x.CurrentUserId,
                            x.CommentId,
                            x.RegionOfInterest,
                            x.Settings
                        }).ToList();

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
                            Settings = series.Settings
                        };

                        if (seriesToAdd.CommentId != null)
                            seriesToAdd.Comment = GetCommentById(seriesToAdd.CommentId.Value);

                        seriesToAdd.ReferencePhotoForSeries = GetReferencePhotoById(seriesToAdd.SeriesId, seriesToAdd);
                        seriesToAdd.MeasurementsSeries = GetMeasurements(seriesToAdd.SeriesId, seriesToAdd).OrderBy(x => x.MeasurementOrderInSeries).ToList();
                        seriesToAdd.Substance = GetSubstanceById(seriesToAdd.SeriesId, seriesToAdd);
                        seriesToAdd.ThermalPlot = GetSeriesPlot(seriesToAdd.SeriesId, seriesToAdd);

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

        private List<DbMeasurement> GetMeasurements(Guid measurementId, DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var measurementForSeries = context.Measurements.Where(x => x.CurrentSeriesId == measurementId)
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
                        x.FrontDropPhotoId,
                        x.SideDropPhotoId,
                        x.Drop,
                        x.CommentId
                    }).ToList();

                var dbMeasurementsForAdd = new List<DbMeasurement>();

                foreach (var measurement in measurementForSeries)
                {
                    var measurementToAdd = new DbMeasurement
                    {
                        AddedDate = measurement.AddedDate,
                        CurrentSeries = series,
                        FrontDropPhotoId = measurement.FrontDropPhotoId,
                        FrontDropPhoto = measurement.FrontDropPhotoId.HasValue
                            ? GetDbDropPhotoById(measurement.FrontDropPhotoId.Value)
                            : null,
                        SideDropPhotoId = measurement.SideDropPhotoId,
                        SideDropPhoto = measurement.SideDropPhotoId.HasValue
                            ? GetDbDropPhotoById(measurement.SideDropPhotoId.Value)
                            : null,
                        CurrentSeriesId = measurement.CurrentSeriesId,
                        MeasurementId = measurement.MeasurementId,
                        Name = measurement.Name,
                        CreationDateTime = measurement.CreationDateTime,
                        MeasurementOrderInSeries = measurement.MeasurementOrderInSeries,
                        AmbientTemperature = measurement.AmbientTemperature,
                        CommentId = measurement.CommentId
                    };

                    measurementToAdd.ThermalPhoto = GetDbThermalPhoto(measurementToAdd);
                    if (measurementToAdd.CommentId != null)
                        measurementToAdd.Comment = GetCommentById(measurementToAdd.CommentId.Value);

                    var dropToAdd = context.Drops.FirstOrDefault(x => x.DropId == measurement.Drop.DropId);

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

        private DbReferencePhoto GetReferencePhotoById(Guid referencePhotoId, DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var referencePhotoForSeries = context.ReferencePhotos
                    .Where(x => x.Series.SeriesId == referencePhotoId)
                    .Select(x => new
                    {
                        x.Name,
                        x.PixelsInMillimeter,
                        x.ReferenceLine,
                        x.PhotoId,
                        x.Series,
                        x.AddedDate,
                        x.CreationDateTime,
                        x.PhotoType
                    }).FirstOrDefault();

                if (referencePhotoForSeries == null)
                {
                    return null;
                }

                return new DbReferencePhoto
                {
                    Name = referencePhotoForSeries.Name,
                    PixelsInMillimeter = referencePhotoForSeries.PixelsInMillimeter,
                    PhotoId = referencePhotoForSeries.PhotoId,
                    ReferenceLine = referencePhotoForSeries.ReferenceLine,
                    Series = series,
                    AddedDate = referencePhotoForSeries.AddedDate,
                    CreationDateTime = referencePhotoForSeries.CreationDateTime
                };
            }
        }

        private DbPlot GetSeriesPlot(Guid seriesId, DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var seriesPlot = context.Plots
                    .Where(x => x.Series.SeriesId == seriesId)
                    .Select(x => new
                    {
                        x.Name,
                        x.Points,
                        x.CurrentUser,
                        x.CurrentUserId,
                        x.PlotId,
                        x.PlotType,
                        x.Settings,
                    }).FirstOrDefault();

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
                    CurrentUser = seriesPlot.CurrentUser,
                    PlotType = seriesPlot.PlotType,
                    PlotId = seriesPlot.PlotId,
                    Settings = seriesPlot.Settings
                };
            }
        }

        private DbSubstances GetSubstanceById(Guid substanceId, DbSeries series)
        {
            using (var context = new DDropContext())
            {
                var substanceForSeries = context.Substances
                    .Where(x => x.Series.SeriesId == substanceId)
                    .Select(x => new
                    {
                        x.Id,
                        x.Series,
                        x.SubstanceId,
                        x.CommonName
                    }).FirstOrDefault();

                if (substanceForSeries == null)
                {
                    return null;
                }

                return new DbSubstances
                {
                    Id = substanceForSeries.Id,
                    SubstanceId = substanceForSeries.SubstanceId,
                    Series = series,
                    CommonName = substanceForSeries.CommonName
                };
            }
        }

        private DbComment GetCommentById(Guid commentId)
        {
            using (var context = new DDropContext())
            {
                var comment = context.Comments
                    .Where(x => x.CommentId == commentId)
                    .Select(x => new
                    {
                        x.CommentId,
                        x.Content,
                        x.Type
                    }).FirstOrDefault();

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

        private DbDropPhoto GetDbDropPhotoById(Guid photoId)
        {
            using (var context = new DDropContext())
            {
                var dropPhoto = context.DropPhotos
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
                    }).FirstOrDefault();

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
                    dbDropPhoto.Comment = GetCommentById(dbDropPhoto.CommentId.Value);
                }

                if (dbDropPhoto.ContourId != null)
                {
                    dbDropPhoto.Contour = GetContourById(dbDropPhoto.ContourId.Value);
                }

                return dbDropPhoto;
            }
        }

        private DbThermalPhoto GetDbThermalPhoto(DbMeasurement measurement)
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
                    dbThermalPhoto.Comment = GetCommentById(dbThermalPhoto.CommentId.Value);

                return dbThermalPhoto;
            }
        }

        private DbContour GetContourById(Guid contourId)
        {
            using (var context = new DDropContext())
            {
                var contour = context.Contours.FirstOrDefault(x => x.ContourId == contourId);

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

                    var referencePhotoForSeries = context.ReferencePhotos.Where(x => x.Series.SeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Name,
                            x.PixelsInMillimeter,
                            x.ReferenceLine,
                            x.CreationDateTime,
                            x.AddedDate,
                            x.PhotoType,
                            x.Content
                        }).FirstOrDefault();

                    var measurementForSeries = context.Measurements.Where(x => x.CurrentSeriesId == seriesId)
                        .Select(x => new
                        {
                            x.Name,
                            x.CreationDateTime,
                            x.MeasurementOrderInSeries,
                            x.MeasurementId,
                            x.AddedDate,
                            x.AmbientTemperature,
                            x.FrontDropPhotoId,
                            x.SideDropPhotoId
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
                        };

                        if (measurement.FrontDropPhotoId != null)
                        {
                            var frontDropPhoto = await context.DropPhotos.Where(x => x.PhotoId == measurement.FrontDropPhotoId).Select(x => new
                            {
                                x.VerticalLine,
                                x.HorizontalLine,
                                x.XDiameterInPixels,
                                x.Name,
                                x.PhotoType,
                                x.YDiameterInPixels,
                                x.CreationDateTime,
                                x.ZDiameterInPixels,
                                x.PhotoId
                            }).FirstOrDefaultAsync();
                            
                            if (frontDropPhoto != null)
                            {
                                var frontDropPhotoForAdd = new DbDropPhoto()
                                {
                                    XDiameterInPixels = frontDropPhoto.XDiameterInPixels,
                                    YDiameterInPixels = frontDropPhoto.YDiameterInPixels,
                                    ZDiameterInPixels = frontDropPhoto.ZDiameterInPixels,
                                    VerticalLine = frontDropPhoto.VerticalLine,
                                    CreationDateTime = frontDropPhoto.CreationDateTime,
                                    HorizontalLine = frontDropPhoto.HorizontalLine,
                                    Name = frontDropPhoto.Name,
                                    PhotoType = frontDropPhoto.PhotoType,
                                    PhotoId = frontDropPhoto.PhotoId,
                                };

                                var contour = await context.Contours.Where(x => x.ContourId == frontDropPhoto.PhotoId)
                                    .Select(x => new
                                    {
                                        x.CalculationParameters,
                                        x.CalculationProvider,
                                        x.ConnectedLines
                                    }).FirstOrDefaultAsync();

                                if (contour != null)
                                {
                                    frontDropPhotoForAdd.Contour = new DbContour
                                    {
                                        CalculationParameters = contour.CalculationParameters,
                                        CalculationProvider = contour.CalculationProvider,
                                        ConnectedLines = contour.ConnectedLines
                                    };
                                }

                                measurementForAdd.FrontDropPhoto = frontDropPhotoForAdd;
                            }
                        }

                        if (measurement.SideDropPhotoId != null)
                        {
                            var sideDropPhoto = await context.DropPhotos.Where(x => x.PhotoId == measurement.FrontDropPhotoId).Select(x => new
                            {
                                x.VerticalLine,
                                x.HorizontalLine,
                                x.XDiameterInPixels,
                                x.Name,
                                x.PhotoType,
                                x.YDiameterInPixels,
                                x.CreationDateTime,
                                x.ZDiameterInPixels,
                                x.PhotoId
                            }).FirstOrDefaultAsync();

                            if (sideDropPhoto != null)
                            {
                                var sideDropPhotoForAdd = new DbDropPhoto()
                                {
                                    XDiameterInPixels = sideDropPhoto.XDiameterInPixels,
                                    YDiameterInPixels = sideDropPhoto.YDiameterInPixels,
                                    ZDiameterInPixels = sideDropPhoto.ZDiameterInPixels,
                                    VerticalLine = sideDropPhoto.VerticalLine,
                                    CreationDateTime = sideDropPhoto.CreationDateTime,
                                    HorizontalLine = sideDropPhoto.HorizontalLine,
                                    Name = sideDropPhoto.Name,
                                    PhotoType = sideDropPhoto.PhotoType,
                                    PhotoId = sideDropPhoto.PhotoId,
                                };

                                var contour = await context.Contours.Where(x => x.ContourId == sideDropPhoto.PhotoId)
                                    .Select(x => new
                                    {
                                        x.CalculationParameters,
                                        x.CalculationProvider,
                                        x.ConnectedLines
                                    }).FirstOrDefaultAsync();

                                if (contour != null)
                                {
                                    sideDropPhotoForAdd.Contour = new DbContour
                                    {
                                        CalculationParameters = contour.CalculationParameters,
                                        CalculationProvider = contour.CalculationProvider,
                                        ConnectedLines = contour.ConnectedLines
                                    };
                                }

                                measurementForAdd.SideDropPhoto = sideDropPhotoForAdd;
                            }
                        }

                        var drop = await context.Drops.FirstOrDefaultAsync(x => x.Measurement.MeasurementId == measurement.MeasurementId);
                        if (drop != null) drop.DropId = Guid.Empty;

                        measurementForAdd.Drop = drop;

                        dbMeasurementForAdd.Add(measurementForAdd);
                    }

                    DbReferencePhoto referencePhotoForAdd = null;

                    if (referencePhotoForSeries != null)
                    {
                        referencePhotoForAdd = new DbReferencePhoto
                        {
                            Name = referencePhotoForSeries.Name,
                            PixelsInMillimeter = referencePhotoForSeries.PixelsInMillimeter,
                            ReferenceLine = referencePhotoForSeries.ReferenceLine,
                            AddedDate = referencePhotoForSeries.AddedDate,
                            Content = referencePhotoForSeries.Content,
                            CreationDateTime = referencePhotoForSeries.CreationDateTime,
                            PhotoType = referencePhotoForSeries.PhotoType
                        };
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
                    var dbMeasurement = await context.Measurements.SingleOrDefaultAsync(x => x.MeasurementId == measurement.MeasurementId);

                    if (dbMeasurement != null)
                    {
                        if (dropPhoto.PhotoType == "FrontDropPhoto")
                        {
                            dbMeasurement.FrontDropPhoto = dropPhoto;
                            dbMeasurement.FrontDropPhotoId = dropPhoto.PhotoId;
                        }

                        if (dropPhoto.PhotoType == "SideDropPhoto")
                        {
                            dbMeasurement.SideDropPhoto = dropPhoto;
                            dbMeasurement.SideDropPhotoId = dropPhoto.PhotoId;
                        }
                    }

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

                    if (dropPhoto.PhotoType == "FrontDropPhoto")
                    {
                        var measurement =
                            await context.Measurements.FirstOrDefaultAsync(x => x.FrontDropPhotoId == dropPhoto.PhotoId);

                        if (measurement != null)
                        {
                            measurement.FrontDropPhoto = null;
                            measurement.FrontDropPhotoId = null;
                        }
                    }

                    if (dropPhoto.PhotoType == "SideDropPhoto")
                    {
                        var measurement =
                            await context.Measurements.FirstOrDefaultAsync(x => x.SideDropPhotoId == dropPhoto.PhotoId);

                        if (measurement != null)
                        {
                            measurement.SideDropPhoto = null;
                            measurement.SideDropPhotoId = null;
                        }
                    }

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
                    if (measurement.FrontDropPhotoId != null)
                    {
                        if (measurement.FrontDropPhoto.Contour != null && measurement.FrontDropPhoto.ContourId != null)
                        {
                            await DeleteContour(measurement.FrontDropPhoto.ContourId.Value);
                        }

                        if (measurement.FrontDropPhoto.Comment != null)
                        {
                            await DeleteComment(measurement.FrontDropPhoto.Comment);
                        }

                        await DeleteDropPhoto(measurement.FrontDropPhoto);
                    }

                    if (measurement.SideDropPhotoId != null)
                    {
                        if (measurement.SideDropPhoto.Contour != null && measurement.SideDropPhoto.ContourId != null)
                        {
                            await DeleteContour(measurement.SideDropPhoto.ContourId.Value);
                        }

                        if (measurement.SideDropPhoto.Comment != null)
                        {
                            await DeleteComment(measurement.SideDropPhoto.Comment);
                        }

                        await DeleteDropPhoto(measurement.SideDropPhoto);
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

                    //if (measurement.Drop != null)
                    //{
                    //    await DeleteDrop(measurement.Drop);
                    //}

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

        #endregion


    }
}