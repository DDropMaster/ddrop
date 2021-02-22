using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DDrop.BE.Models
{
    public class SubstanceIdsResponse
    {
        [JsonPropertyName("results")]
        public List<int> Results { get; set; }

        [JsonPropertyName("limitedToMaxAllowed")]
        public bool LimitedToMaxAllowed { get; set; }
    }
}