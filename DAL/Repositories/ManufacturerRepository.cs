using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RetroGaming.DAL.Context;
using RetroGaming.DAL.Entities;
using RetroGaming.DAL.Models;
using RetroGaming.DAL.Repositories.Interfaces;

namespace RetroGaming.DAL.Repositories
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly AppDbContext _context;

        public ManufacturerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Manufacturer>> GetAllAsync()
        {
            var results = await _context.Database
                .SqlQueryRaw<ManufacturerResult>("EXEC sp_GetAllManufacturers")
                .ToListAsync();

            return results.Select(MapToEntity).ToList();
        }

        public async Task<Manufacturer?> GetByIdAsync(int id)
        {
            var param = new SqlParameter("@Id", id);

            var results = await _context.Database
                .SqlQueryRaw<ManufacturerResult>(
                    "EXEC sp_GetManufacturerById @Id", param)
                .ToListAsync();

            var result = results.FirstOrDefault();
            return result == null ? null : MapToEntity(result);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var result = await _context.Database
                .SqlQueryRaw<bool>(
                    "EXEC sp_ManufacturerNameExists @Name, @ExcludeId",
                    new SqlParameter("@Name", name),
                    new SqlParameter("@ExcludeId", (object?)excludeId ?? DBNull.Value))
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<bool> HasConsolesAsync(int id)
        {
            var result = await _context.Database
                .SqlQueryRaw<bool>(
                    "EXEC sp_ManufacturerHasConsoles @Id",
                    new SqlParameter("@Id", id))
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<Manufacturer> CreateAsync(Manufacturer manufacturer)
        {
            var newId = await _context.Database
                .SqlQueryRaw<int>(
                    "EXEC sp_CreateManufacturer @Name, @Country, @City, @FoundedYear, @Latitude, @Longitude",
                    new SqlParameter("@Name", manufacturer.Name),
                    new SqlParameter("@Country", manufacturer.Country),
                    new SqlParameter("@City", manufacturer.City),
                    new SqlParameter("@FoundedYear", manufacturer.FoundedYear),
                    new SqlParameter("@Latitude", (object?)manufacturer.Latitude ?? DBNull.Value),
                    new SqlParameter("@Longitude", (object?)manufacturer.Longitude ?? DBNull.Value))
                .ToListAsync();

            manufacturer.Id = newId.FirstOrDefault();
            return manufacturer;
        }

        public async Task<Manufacturer> UpdateAsync(Manufacturer manufacturer)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateManufacturer @Id, @Name, @Country, @City, @FoundedYear, @Latitude, @Longitude",
                new SqlParameter("@Id", manufacturer.Id),
                new SqlParameter("@Name", manufacturer.Name),
                new SqlParameter("@Country", manufacturer.Country),
                new SqlParameter("@City", manufacturer.City),
                new SqlParameter("@FoundedYear", manufacturer.FoundedYear),
                new SqlParameter("@Latitude", (object?)manufacturer.Latitude ?? DBNull.Value),
                new SqlParameter("@Longitude", (object?)manufacturer.Longitude ?? DBNull.Value));

            return manufacturer;
        }

        public async Task DeleteAsync(Manufacturer manufacturer)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_DeleteManufacturer @Id",
                new SqlParameter("@Id", manufacturer.Id));
        }

        private static Manufacturer MapToEntity(ManufacturerResult r) => new()
        {
            Id = r.Id,
            Name = r.Name,
            Country = r.Country,
            City = r.City,
            FoundedYear = r.FoundedYear,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            ConsoleCount = r.ConsoleCount
        };
    }
}