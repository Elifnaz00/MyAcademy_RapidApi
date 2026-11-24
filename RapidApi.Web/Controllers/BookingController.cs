using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RapidApi.Web.Models;
using RapidApi.Web.Services.Abstract;


namespace RapidApi.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IValidator<BookingSearchViewModel> _validator;
        private readonly ILocationService _locationService;
        private readonly IBookingService _bookingService;

        public BookingController(
            IValidator<BookingSearchViewModel> validator,
            ILocationService locationService,
            IBookingService hotelService)
        {
            _validator = validator;
            _locationService = locationService;
            _bookingService = hotelService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new BookingSearchViewModel
            {
                City = "Milano, İtalya",
                CheckInDate = new DateTime(2026, 11, 25),
                CheckOutDate = new DateTime(2026, 11, 28),
                Adult = 2,
                Child = 0,
                Rooms = 1,
                Currency = "EUR"
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult HotelList()
        {

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> HotelList(BookingSearchViewModel bookingSearchViewModel)
        {
            var validationResult = await _validator.ValidateAsync(
                bookingSearchViewModel);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(
                        error.PropertyName,
                        error.ErrorMessage);
                }

                return View("Index", bookingSearchViewModel);
            }

            var location = await _locationService.GetLocationByCityAsync(
                bookingSearchViewModel.City!);

            if (location == null || location.DestId == 0)
            {
                return NotFound("Girilen şehir için lokasyon bulunamadı.");
            }

            var hotels = await _bookingService.SearchHotelsAsync(
                bookingSearchViewModel,
                location.DestId);

            ViewBag.CheckIn = bookingSearchViewModel.CheckInDate?.ToString("yyyy-MM-dd");
            ViewBag.CheckOut = bookingSearchViewModel.CheckOutDate?.ToString("yyyy-MM-dd");

            return View(hotels);
        }


        [HttpGet]
        public async Task<IActionResult> HotelDetail(string contentId,  string checkIn, string checkOut)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                return BadRequest("Otel bilgisi bulunamadı.");
            }

            var model = await _bookingService.HotelDetailGetById(
                contentId,
                checkIn,
                checkOut);

            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;

            return View(model);
        }
    }
}
