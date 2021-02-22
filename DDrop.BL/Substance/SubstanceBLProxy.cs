using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DDrop.BE.Models;
using DDrop.Utility.SeriesLocalStorageOperations;
using DDrop.Utility.SettingsRepository;
using IMapper = AutoMapper.IMapper;

namespace DDrop.BL.Substance
{
    public class SubstanceBLProxy : ISubstanceBLProxy
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IMapper _mapper;

        private Uri ApiUri => new Uri(_settingsRepository.SubstanceCatalogUrl);

        private Uri ActionUri(string name) => new Uri(ApiUri, $"{name}");

        public SubstanceBLProxy(ISettingsRepository settingsRepository, IMapper mapper)
        {
            _settingsRepository = settingsRepository;
            _mapper = mapper;
        }

        public async Task<List<SubstanceModel>> GetSearchResults(SubstanceQueryIdRequest queryIdRequest)
        {
            var response = await GetQueryIdByName(queryIdRequest);

            if (response != null)
            {
                var substancesIds = await GetSubstances(response.QueryId);

                if (substancesIds.Results.Count > 0)
                {
                    List<SubstanceModel> substances = new List<SubstanceModel>();

                    foreach (var substancesId in substancesIds.Results)
                    {
                        var substance = await GetSubstance(substancesId);

                        if (substance != null)
                        {
                            substances.Add(_mapper.Map<SubstanceDetailsResponse, SubstanceModel>(substance));
                        }
                    }

                    return substances;
                }
            }

            return null;
        }

        private async Task<SubstanceQueryIdResponse> GetQueryIdByName(SubstanceQueryIdRequest queryIdRequest)
        {
            var json = JsonSerializeProvider.SerializeToString(queryIdRequest);

            var response =  await PostRequest($"filter/name/", json);

            var queryIdResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializeProvider.DeserializeFromString<SubstanceQueryIdResponse>(queryIdResponse);
        }

        private async Task<SubstanceIdsResponse> GetSubstances(Guid queryId)
        {
            return await GetRequest<SubstanceIdsResponse>($"filter/{queryId}/results");
        }

        private async Task<SubstanceDetailsResponse> GetSubstance(int substanceId)
        {
            return await GetRequest<SubstanceDetailsResponse>($"records/{substanceId}/details?fields=CommonName");
        }

        private AuthenticationHeaderValue GetAuthenticationHeader()
        {
            return new AuthenticationHeaderValue("apikey", _settingsRepository.SubstanceCatalogApiKey);
        }

        private async Task<HttpResponseMessage> PostRequest(string name, string data)
        {
            using (var client = new HttpClient())
            {
                var authHeader = GetAuthenticationHeader();

                client.DefaultRequestHeaders.Add(authHeader.Scheme, authHeader.Parameter);
                try
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    return await client.PostAsync(ActionUri(name), content);
                }
                catch (HttpRequestException e)
                {
                    throw new AccessViolationException();
                }
            }
        }

        private async Task<T> GetRequest<T>(string name)
        {
            using (var client = new HttpClient())
            {
                var authHeader = GetAuthenticationHeader();

                client.DefaultRequestHeaders.Add(authHeader.Scheme, authHeader.Parameter);
                try
                {
                    var responseBody = await client.GetStringAsync(ActionUri(name));

                    return JsonSerializeProvider.DeserializeFromString<T>(responseBody);
                }
                catch (HttpRequestException e)
                {
                    throw new AccessViolationException();
                }
            }
        }
    }
}