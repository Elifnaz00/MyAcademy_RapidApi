using System.Text.Json.Serialization;

namespace RapidApi.Web.Models.Dtos
{
    public class LocationResponseDto
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public List<LocationDto> Data { get; set; }
    }
}
