using System.ComponentModel.DataAnnotations;

namespace RetroGaming.Common.DTOs
{
    public class ConsoleDto
    {
        public int Id { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public int Generation { get; set; }
        public decimal? UnitsSoldMillions { get; set; }
        public bool IsDiscontinued { get; set; }
    }

    public class CreateConsoleDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a manufacturer.")]
        public int ManufacturerId { get; set; }

        [Required(ErrorMessage = "Console name is required.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1800, 2100)]
        public int ReleaseYear { get; set; }

        [Range(1, 20)]
        public int Generation { get; set; }

        [Range(0, 10000)]
        public decimal? UnitsSoldMillions { get; set; }

        public bool IsDiscontinued { get; set; }
    }

    public class UpdateConsoleDto : CreateConsoleDto { }
}