using System.Threading.Tasks;
using System.Windows.Controls;
using DDrop.Models;

namespace DDrop.Logic.SeriesLogic
{
    public interface ISeriesLogic
    {
        Task DeleteSeries(SeriesView series, SeriesView currentSeries, Canvas canvas);
    }
}