namespace RetroGaming.DAL.Models
{
    public class ConsoleResult
    {
        public int Id { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public int Generation { get; set; }
        public decimal? UnitsSoldMillions { get; set; }
        public bool IsDiscontinued { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}