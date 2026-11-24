using System.Text.Json.Serialization;

namespace RapidApi.Web.Models.Dtos
{
    public class LocationDto
    {
        [JsonPropertyName("geoId")]
        public long DestId { get; set; }

      
    }
}
