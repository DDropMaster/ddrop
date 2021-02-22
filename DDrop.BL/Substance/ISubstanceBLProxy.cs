using System.Collections.Generic;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.Substance
{
    public interface ISubstanceBLProxy
    {
        Task<List<SubstanceModel>> GetSearchResults(SubstanceQueryIdRequest queryIdRequest);
    }
}