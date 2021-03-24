using System.Threading.Tasks;
using System.Windows.Controls;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.BL.Series;
using DDrop.Models;

namespace DDrop.Logic.SeriesLogic
{
    public class SeriesLogic : ISeriesLogic
    {
        private readonly ISeriesBL _seriesBl;
        private readonly IMapper _mapper;

        public SeriesLogic(ISeriesBL seriesBl, IMapper mapper)
        {
            _seriesBl = seriesBl;
            _mapper = mapper;
        }

        public async Task DeleteSeries(SeriesView series, SeriesView currentSeries, Canvas canvas)
        {
            var serie = _mapper.Map<SeriesView, Series>(series);
            var currentSerie = _mapper.Map<SeriesView, Series>(currentSeries);

            await _seriesBl.DeleteSeries(serie, currentSerie);

            if (series == currentSeries)
            {
                foreach (var currentSerieReferencePhotoForSeries in series.ReferencePhotoForSeries)
                {
                    if (currentSerieReferencePhotoForSeries?.Line != null)
                        canvas.Children.Remove(currentSerieReferencePhotoForSeries?.Line);
                }

                currentSeries.Title = null;
            }
        }
    }
}