using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.BL.Drop
{
    public class DropBL : IDropBL
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public DropBL(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task UpdateDrop(BE.Models.Drop drop)
        {
            var dbDrop = _mapper.Map<BE.Models.Drop, DbDrop>(drop);

            await Task.Run(() => _dDropRepository.UpdateDrop(dbDrop));
        }

        public async Task UpdateDropTemperature(double value, Guid dropId)
        {
            await Task.Run(() => _dDropRepository.UpdateDropTemperature(value, dropId));
        }
    }
}
