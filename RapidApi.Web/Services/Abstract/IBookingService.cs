using RapidApi.Web.Models;

namespace RapidApi.Web.Services.Abstract
{
    public interface IBookingService
    {
        Task<List<HotelViewModel>> SearchHotelsAsync(BookingSearchViewModel model, long destId);

        Task<HotelDetailModel> HotelDetailGetById(string contentId, string checkIn, string checkOut);


    }
}
