namespace RetroGaming.DAL.Entities
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int FoundedYear { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Populated by stored procedures via ManufacturerResult.
        public int ConsoleCount { get; set; }

        public ICollection<GameConsole> Consoles { get; set; } = new List<GameConsole>();
    }
}