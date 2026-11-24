namespace RapidApi.Web.Models
{
    public class HotelDetailModel
    {
        public string HotelName { get; set; } = string.Empty;

        public decimal? ReviewScore { get; set; }

        public int? ReviewCount { get; set; }

        public string Description { get; set; } = string.Empty;

        public List<string> Facilities { get; set; } = new();

        public List<string> Tags { get; set; } = new();

        public List<string> PhotoUrls { get; set; } = new();
    }
}
