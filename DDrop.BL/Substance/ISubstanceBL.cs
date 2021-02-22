using System.Collections.Generic;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.Substance
{
    public interface ISubstanceBL
    {
        Task<List<SubstanceModel>> GetSearchResults(SubstanceQueryIdRequest queryIdRequest);
        Task SaveSubstance(SubstanceModel substance);
        Task DeleteSubstance(SubstanceModel substance);
    }
}