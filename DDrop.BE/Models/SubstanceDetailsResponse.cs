using System.Text.Json.Serialization;

namespace DDrop.BE.Models
{
    public class SubstanceDetailsResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("commonName")]
        public string CommonName { get; set; }
    }
}