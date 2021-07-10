using System;
using System.IO;
using System.Threading.Tasks;
using DDrop.Db.DbEntities;

namespace DDrop.BL.ExportBL
{
    public interface IExportBl
    {
        Task<DbSeries> ImportLocalSeriesAsync(Stream content, Guid userId);
        Task ExportLocalSeriesAsync(Guid seriesId, string path);
    }
}