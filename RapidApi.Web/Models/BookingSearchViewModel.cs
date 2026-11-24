namespace RapidApi.Web.Models
{
    public class BookingSearchViewModel
    {
        public string? City { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
        public int Rooms { get; set; }
        public string? Currency { get;set; } //para birimi
        public List<int> ChildAges { get; set; } = new();
        public string? SearchType { get; set; }
    }
}
