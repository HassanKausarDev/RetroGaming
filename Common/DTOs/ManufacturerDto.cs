using System.ComponentModel.DataAnnotations;

namespace RetroGaming.Common.DTOs
{
    public class ManufacturerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int FoundedYear { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int ConsoleCount { get; set; }
    }

    public class CreateManufacturerDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Range(1800, 2100)]
        public int FoundedYear { get; set; }

        [Range(-90.0, 90.0)]
        public decimal? Latitude { get; set; }

        [Range(-180.0, 180.0)]
        public decimal? Longitude { get; set; }
    }

    public class UpdateManufacturerDto : CreateManufacturerDto { }

    public class ManufacturerMapDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int ConsoleCount { get; set; }
    }
}