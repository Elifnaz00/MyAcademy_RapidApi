using RapidApi.Web.Exceptions;
using RapidApi.Web.Models.Dtos;
using RapidApi.Web.Services.Abstract;
using System.Text.Json;

namespace RapidApi.Web.Services.Concrete
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationService> _logger;

        public LocationService(
               HttpClient httpClient,
               ILogger<LocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<LocationDto?> GetLocationByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return null;
            }
            try
            {
                var encodedCity = Uri.EscapeDataString(city);
                var locationRequestUrl = $"https://tripadvisor-com1.p.rapidapi.com/auto-complete?query={encodedCity}";

                using var response = await _httpClient.GetAsync(locationRequestUrl);

                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<LocationResponseDto>(
                   body,
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   });

                return result?.Data?.FirstOrDefault();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Location API zaman aşımına uğradı.");

                throw new ExternalApiTimeoutException(
                    "Lokasyon servisi zamanında yanıt vermedi.",
                    ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Location API bağlantı hatası.");

                throw new ExternalApiException(
                    "Lokasyon servisine bağlanılamadı.",
                    ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Location API JSON çözümleme hatası.");

                throw new ExternalApiResponseException(
                    "Lokasyon servisinden geçersiz yanıt alındı.",
                    ex);
            }
        }

    }
    }
}
