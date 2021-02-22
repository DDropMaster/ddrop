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
    public class DbUserPlotsResolver : IValueResolver<User, DbUser, List<DbPlot>>
    {
        public List<DbPlot> Resolve(User source, DbUser destination, List<DbPlot> destMember, ResolutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}