using RapidApi.Web.Exceptions;
using RapidApi.Web.Models;
using RapidApi.Web.Models.Dtos;
using RapidApi.Web.Services.Abstract;
using System.Linq;
using System.Text.Json;

namespace RapidApi.Web.Services.Concrete
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationService> _logger;

        public BookingService(HttpClient httpClient, ILogger<LocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HotelDetailModel> HotelDetailGetById(string contentId, string checkIn, string checkOut)
        {

            try
            {
                var hotelDetailRequestUrl = new Uri($"https://tripadvisor-com1.p.rapidapi.com/hotels/details?contentId={contentId}&checkIn={checkIn}&checkOut={checkOut}")


            using var response = await _httpClient.GetAsync(hotelDetailRequestUrl);

                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var apiResponse = JsonSerializer.Deserialize<HotelDetailResponseDto>(
                   body,
                   options);
                if (apiResponse?.Data == null)
                {
                    throw new Exception("Otel detay verisi alınamadı.");
                }

                var sections = apiResponse.Data.Sections ?? new List<JsonElement>();

                // Otel genel bilgilerinin bulunduğu bölüm
                var overviewSection = FindSection(
                    sections,
                    "AppPresentation_PoiOverview");

                // Otel hakkında bilgilerin bulunduğu bölüm
                var aboutSection = FindSection(
                    sections,
                    "AppPresentation_PoiAbout");

                // Otel fotoğraflarının bulunduğu bölüm
                var heroSection = FindSection(
                    sections,
                    "AppPresentation_PoiHeroStandard");

                var hotelDetail = new HotelDetailModel
                {
                    HotelName = GetString(overviewSection, "name")
                                ?? apiResponse.Data.Container?.NavTitle
                                ?? string.Empty,

                    ReviewScore = GetDecimal(overviewSection, "rating"),

                    ReviewCount = GetInt(overviewSection, "numberReviews"),

                    Description = GetDescription(aboutSection),

                    Facilities = GetFacilities(aboutSection),

                    Tags = GetTags(aboutSection),

                    PhotoUrls = GetPhotoUrls(heroSection)
                };

                return hotelDetail;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Hotel Detail API zaman aşımına uğradı.");

                throw new ExternalApiTimeoutException(
                    "Otel detay servisi zamanında yanıt vermedi.",
                    ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Hotel Detail API bağlantı hatası.");

                throw new ExternalApiException(
                    "Otel detay servisine bağlanılamadı.",
                    ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Hotel Detail API JSON çözümleme hatası.");

                throw new ExternalApiResponseException(
                    "Otel detay servisinden geçersiz yanıt alındı.",
                    ex);
            }



        }

        // İstenen typename değerine sahip bölümü bulur.
        private static JsonElement FindSection(
            List<JsonElement> sections,
            string typeName)
        {
            return sections.FirstOrDefault(section =>
                section.ValueKind == JsonValueKind.Object &&
                section.TryGetProperty("__typename", out var type) &&
                type.GetString() == typeName);
        }

        // JSON içerisinden string değer okur.
        private static string? GetString(
            JsonElement element,
            string propertyName)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(propertyName, out var property) &&
                property.ValueKind == JsonValueKind.String)
            {
                return property.GetString();
            }

            return null;
        }

        // JSON içerisinden decimal değer okur.
        private static decimal? GetDecimal(
            JsonElement element,
            string propertyName)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(propertyName, out var property) &&
                property.ValueKind == JsonValueKind.Number &&
                property.TryGetDecimal(out var value))
            {
                return value;
            }

            return null;
        }

        // JSON içerisinden int değer okur.
        private static int? GetInt(
            JsonElement element,
            string propertyName)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(propertyName, out var property) &&
                property.ValueKind == JsonValueKind.Number &&
                property.TryGetInt32(out var value))
            {
                return value;
            }

            return null;
        }

        // Otel açıklamasını alır.
        private static string GetDescription(JsonElement aboutSection)
        {
            if (!aboutSection.TryGetProperty("nullableContent", out var content) ||
                content.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            foreach (var item in content.EnumerateArray())
            {
                var typeName = GetString(item, "__typename");

                if (typeName == "AppPresentation_CollapsibleTextSubsection" &&
                    item.TryGetProperty("text", out var text))
                {
                    return GetString(text, "string") ?? string.Empty;
                }
            }

            return string.Empty;
        }

        // Otelin tesis özelliklerini alır.
        private static List<string> GetFacilities(JsonElement aboutSection)
        {
            var facilities = new List<string>();

            if (!aboutSection.TryGetProperty("nullableContent", out var content) ||
                content.ValueKind != JsonValueKind.Array)
            {
                return facilities;
            }

            foreach (var item in content.EnumerateArray())
            {
                var typeName = GetString(item, "__typename");

                if (typeName == "AppPresentation_SmallTextListSubsection" &&
                    item.TryGetProperty("list", out var list) &&
                    list.ValueKind == JsonValueKind.Array)
                {
                    foreach (var facility in list.EnumerateArray())
                    {
                        var facilityName = GetString(facility, "string");

                        if (!string.IsNullOrWhiteSpace(facilityName))
                        {
                            facilities.Add(facilityName);
                        }
                    }

                    break;
                }
            }

            return facilities;
        }

        // Otelin etiketlerini alır.
        private static List<string> GetTags(JsonElement aboutSection)
        {
            var tags = new List<string>();

            if (!aboutSection.TryGetProperty("tagsSubsection", out var tagSection) ||
                !tagSection.TryGetProperty("tagsV2", out var tagList) ||
                tagList.ValueKind != JsonValueKind.Array)
            {
                return tags;
            }

            foreach (var item in tagList.EnumerateArray())
            {
                if (item.TryGetProperty("tag", out var tag))
                {
                    var tagName = GetString(tag, "localizedName");

                    if (!string.IsNullOrWhiteSpace(tagName))
                    {
                        tags.Add(tagName);
                    }
                }
            }

            return tags;
        }

        // Otelin fotoğraf URL'lerini alır.
        private static List<string> GetPhotoUrls(JsonElement heroSection)
        {
            var photoUrls = new List<string>();

            if (!heroSection.TryGetProperty("heroContent", out var heroContent) ||
                heroContent.ValueKind != JsonValueKind.Array)
            {
                return photoUrls;
            }

            foreach (var media in heroContent.EnumerateArray())
            {
                if (!media.TryGetProperty("data", out var data) ||
                    !data.TryGetProperty("sizes", out var sizes) ||
                    sizes.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                string? photoUrl = null;
                int largestWidth = 0;

                foreach (var size in sizes.EnumerateArray())
                {
                    if (!size.TryGetProperty("width", out var widthProperty) ||
                        !widthProperty.TryGetInt32(out var width) ||
                        !size.TryGetProperty("url", out var urlProperty))
                    {
                        continue;
                    }

                    var url = urlProperty.GetString();

                    if (width > largestWidth && !string.IsNullOrWhiteSpace(url))
                    {
                        largestWidth = width;
                        photoUrl = url;
                    }
                }

                if (!string.IsNullOrWhiteSpace(photoUrl))
                {
                    photoUrls.Add(photoUrl);
                }
            }

            return photoUrls.Distinct().ToList();
        }

        public async Task<List<HotelViewModel>> SearchHotelsAsync(BookingSearchViewModel model, long destId)
        {
            try
            {
                var childrenAge = string.Join(",", model.ChildAges);

                var arrivalDate = model.CheckInDate?.ToString("yyyy-MM-dd");
                var departureDate = model.CheckOutDate?.ToString("yyyy-MM-dd");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(
                        $"https://tripadvisor-com1.p.rapidapi.com/hotels/search?geoId={destId}&checkIn={arrivalDate}&checkOut={departureDate}&currency={model.Currency}&rooms={model.Rooms}&adults={model.Adult}&children={childrenAge}")
                };

                using var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ExternalApiException(
                        $"Hotel Search API isteği başarısız oldu. HTTP: {(int)response.StatusCode}");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = JsonSerializer.Deserialize<HotelApiResponseDto>(
                    body,
                    options);

                var hotels = apiResponse?.Data?.Hotels?
                    .Select(hotel => new HotelViewModel
                    {
                        Name = hotel.CardTitle?.String ?? "İsimsiz Otel",

                        Price = hotel.CommerceInfo?.PriceForDisplay?.String
                                ?? "Fiyat bilgisi yok",

                        Rating = hotel.BubbleRating?.Rating ?? 0,

                        ReviewCount = hotel.BubbleRating?.NumberReviews?.String
                                      ?? "Henüz değerlendirme yok",

                        ContentId = hotel.CardLink?.Route?.TypedParams?.ContentId ?? "",

                        HotelId = hotel.SaveId?.Id ?? "",

                        PhotoUrl = hotel.CardPhotos?
                            .FirstOrDefault()?.Sizes?.UrlTemplate?
                            .Replace("{width}", "600")
                            .Replace("{height}", "400")
                            ?? ""
                    })
                    .ToList() ?? new List<HotelViewModel>();

                return hotels;
            }
            catch (TaskCanceledException ex)
            {
                throw new ExternalApiTimeoutException(
                    "Otel arama servisi zamanında yanıt vermedi.",
                    ex);
            }
            catch (HttpRequestException ex)
            {
                throw new ExternalApiException(
                    "Otel arama servisine bağlanılamadı.",
                    ex);
            }
            catch (JsonException ex)
            {
                throw new ExternalApiResponseException(
                    "Otel arama servisinden geçersiz yanıt alındı.",
                    ex);
            }


        }
    }
}
