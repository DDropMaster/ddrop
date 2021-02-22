using System.Collections.Generic;
using System.Linq;
using DDrop.BE.Models;
using DDrop.Controls.AutoComplete;

namespace DDrop.BL.Substance
{
    public class SubstancesDataProvider : ISearchDataProvider
    {
        private readonly ISubstanceBL _substanceBl;
        private SearchResult result;


        public SubstancesDataProvider(ISubstanceBL substanceBl)
        {
            _substanceBl = substanceBl;
        }

        public SearchResult DoSearch(string searchTerm)
        {
            var searchResults = _substanceBl.GetSearchResults(new SubstanceQueryIdRequest() { Name = searchTerm }).Result;

            result = new SearchResult();
            result.SearchTerm = searchTerm;
            result.Results = new Dictionary<object, string>();

            if (searchResults == null)
            {
                result.SearchTerm = "Нет результатов";
                return result;
            }

            foreach (var searchResult in searchResults)
            {
                result.Results.Add(searchResult.Id, searchResult.CommonName);
            }

            return result;
        }

        public SearchResult SearchByKey(object Key)
        {
            return result;
        }
    }
}