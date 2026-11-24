using System.Text.Json.Serialization;

namespace RapidApi.Web.Models.Dtos
{
    public class HotelApiResponseDto
    {
        [JsonPropertyName("data")]
        public HotelDataDto Data { get; set; }
    }
    public class HotelDataDto
    {
        [JsonPropertyName("hotels")]
        public List<HotelCardDto> Hotels { get; set; }
    }

    public class HotelCardDto
    {
        [JsonPropertyName("cardTitle")]
        public LocalizedStringDto CardTitle { get; set; }

        [JsonPropertyName("bubbleRating")]
        public BubbleRatingDto BubbleRating { get; set; }

        [JsonPropertyName("commerceInfo")]
        public HotelCommerceInfoDto CommerceInfo { get; set; }

        [JsonPropertyName("cardPhotos")]
        public List<HotelPhotoDto> CardPhotos { get; set; }

        [JsonPropertyName("saveId")]
        public HotelSaveIdDto SaveId { get; set; }

        [JsonPropertyName("cardLink")]
        public HotelCardLinkDto? CardLink { get; set; }
    }
    public class HotelCardLinkDto
    {
        [JsonPropertyName("route")]
        public RouteDto? Route { get; set; }
    }

    public class RouteDto
    {
        [JsonPropertyName("typedParams")]
        public TypedParamsDto? TypedParams { get; set; }
    }

    public class TypedParamsDto
    {
        [JsonPropertyName("contentId")]
        public string ContentId { get; set; }
    }
    public class LocalizedStringDto
    {
        [JsonPropertyName("string")]
        public string String { get; set; }
    }

    public class BubbleRatingDto
    {
        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("numberReviews")]
        public LocalizedStringDto NumberReviews { get; set; }
    }

    public class HotelCommerceInfoDto
    {
        [JsonPropertyName("priceForDisplay")]
        public LocalizedStringDto PriceForDisplay { get; set; }
    }

    public class HotelPhotoDto
    {
        [JsonPropertyName("sizes")]
        public HotelPhotoSizeDto Sizes { get; set; }
    }

    public class HotelPhotoSizeDto
    {
        [JsonPropertyName("urlTemplate")]
        public string UrlTemplate { get; set; }
    }

    public class HotelSaveIdDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
