namespace RetroGaming.DAL.Models
{
    public class ManufacturerResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int FoundedYear { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int ConsoleCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}