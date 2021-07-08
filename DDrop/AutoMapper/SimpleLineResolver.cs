using AutoMapper;
using DDrop.BE.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.Enums;
using DDrop.Models;
using DDrop.Db.DbEntities;
using System.Collections.Generic;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop.AutoMapper
{
    public class SimpleLineResolver : IValueResolver<DbDropPhoto, DropPhoto, List<SimpleLine>>
    {
        public List<SimpleLine> Resolve(DbDropPhoto source, DropPhoto destination, List<SimpleLine> destMember,
            ResolutionContext context)
        {
            if (source.SimpleLines != null)
            {
                return JsonSerializeProvider.DeserializeFromString<List<SimpleLine>>(source.SimpleLines);
            }


            return null;
        }
    }
}