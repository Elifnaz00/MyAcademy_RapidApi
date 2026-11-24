using RapidApi.Web.Models.Dtos;

namespace RapidApi.Web.Services.Abstract
{
    public interface ILocationService
    {
        Task<LocationDto?> GetLocationByCityAsync(string city);
    }
}
