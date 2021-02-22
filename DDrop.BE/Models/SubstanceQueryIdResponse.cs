using System;
using System.Text.Json.Serialization;

namespace DDrop.BE.Models
{
    public class SubstanceQueryIdResponse
    {
        [JsonPropertyName("queryId")]
        public Guid QueryId { get; set; }
    }
}