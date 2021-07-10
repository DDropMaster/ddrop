using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.BL.Measurement;
using DDrop.BL.Series;
using DDrop.Models;

namespace DDrop.Logic.SeriesLogic
{
    public class SeriesLogic : ISeriesLogic
    {
        private readonly ISeriesBL _seriesBl;
        private readonly IMapper _mapper;
        private readonly IMeasurementBl _measurementBl;

        public SeriesLogic(ISeriesBL seriesBl, IMapper mapper, IMeasurementBl measurementBl)
        {
            _seriesBl = seriesBl;
            _mapper = mapper;
            _measurementBl = measurementBl;
        }

        public async Task DeleteSeries(SeriesView series, Canvas canvas, ObservableCollection<SeriesView> userSeries)
        {
            var serie = _mapper.Map<SeriesView, Series>(series);

            serie.MeasurementsSeries = await _measurementBl.GetMeasurements(serie.SeriesId);

            await _seriesBl.DeleteSeries(serie);

            if (series.ReferencePhotoForSeries != null)
            {
                foreach (var currentSerieReferencePhotoForSeries in series.ReferencePhotoForSeries)
                {
                    if (currentSerieReferencePhotoForSeries?.Line != null)
                        canvas.Children.Remove(currentSerieReferencePhotoForSeries?.Line);
                }
            }

            userSeries.Remove(series);
            series.Title = null;
        }
    }
}