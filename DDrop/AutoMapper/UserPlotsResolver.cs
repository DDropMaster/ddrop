using System.Collections.Generic;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Db.DbEntities;
using System.Collections.ObjectModel;
using System.Windows;
using DDrop.Utility.SeriesLocalStorageOperations;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace DDrop.AutoMapper
{
    public class UserPlotsResolver : IValueResolver<DbUser, User, ObservableCollection<Plot>>
    {
        public ObservableCollection<Plot> Resolve(DbUser source, User destination, ObservableCollection<Plot> destMember,
            ResolutionContext context)
        {
            if (source.Plots != null)
            {
                var plots = new ObservableCollection<Plot>();
                foreach (var plot in source.Plots)
                {
                    var points = JsonSerializeProvider.DeserializeFromString<ObservableCollection<SimplePoint>>(plot.Points);

                    plots.Add(new Plot
                    {
                        CurrentUser = destination,
                        CurrentUserId = destination.UserId,
                        Name = plot.Name,
                        PlotId = plot.PlotId,
                        Points = points
                    });
                }

                return plots;
            }

            return null;
        }
    }
}