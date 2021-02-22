using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.BL.Substance
{
    public class SubstanceBL : ISubstanceBL
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly ISubstanceBLProxy _substanceBlProxy;
        private readonly IMapper _mapper;

        public SubstanceBL(ISubstanceBLProxy substanceBlProxy, IDDropRepository dDropRepository, IMapper mapper)
        {
            _substanceBlProxy = substanceBlProxy;
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task<List<SubstanceModel>> GetSearchResults(SubstanceQueryIdRequest queryIdRequest)
        {
            return await _substanceBlProxy.GetSearchResults(queryIdRequest);
        }

        public async Task SaveSubstance(SubstanceModel substance)
        {
            await _dDropRepository.UpdateSubstance(_mapper.Map<SubstanceModel, DbSubstances>(substance));
        }

        public async Task DeleteSubstance(SubstanceModel substance)
        {
            await _dDropRepository.DeleteSubstance(substance.SubstanceId);
        }
    }
}