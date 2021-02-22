using System.Text.Json.Serialization;

namespace DDrop.BE.Models
{
    public class SubstanceQueryIdRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}