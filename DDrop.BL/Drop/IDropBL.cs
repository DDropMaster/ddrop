using System;
using System.Threading.Tasks;

namespace DDrop.BL.Drop
{
    public interface IDropBL
    {
        Task UpdateDrop(BE.Models.Drop drop);
        Task UpdateDropTemperature(double value, Guid dropId);
    }
}