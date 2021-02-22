using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.Db.DbEntities;
using DDrop.Utility.SeriesExporter;

namespace DDrop.BL.ExportBL
{
    public class ExportBl : IExportBl
    {
        private readonly IMapper _mapper;

        public ExportBl(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<DbSeries> ImportLocalSeriesAsync(Stream content, BE.Models.User user)
        {
            return await Task.Run(() => SeriesExporter.ImportLocalSeriesAsync(content, _mapper.Map<BE.Models.User, DbUser>(user)));
        }
    }
}