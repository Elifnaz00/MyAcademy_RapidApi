using System.Text.Json;
using System.Text.Json.Serialization;

namespace RapidApi.Web.Models.Dtos
{
    public class HotelDetailResponseDto
    {
        [JsonPropertyName("data")]
        public HotelDetailDataDto Data { get; set; }
    }

    public class HotelDetailDataDto
    {
        [JsonPropertyName("container")]
        public HotelDetailContainerDto Container { get; set; }

        [JsonPropertyName("sections")]
        public List<JsonElement> Sections { get; set; }
    }

    public class HotelDetailContainerDto
    {
        [JsonPropertyName("navTitle")]
        public string NavTitle { get; set; }
    }
}
