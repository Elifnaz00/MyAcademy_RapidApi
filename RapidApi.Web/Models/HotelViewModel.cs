namespace RapidApi.Web.Models
{
    public class HotelViewModel
    {
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Price { get; set; }
        public double Rating { get; set; }
        public string ReviewCount { get; set; }
        public string HotelId { get; set; }

        public string ContentId { get; set; }
    }
}
