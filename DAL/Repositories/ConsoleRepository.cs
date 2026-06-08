using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RetroGaming.DAL.Context;
using RetroGaming.DAL.Entities;
using RetroGaming.DAL.Models;
using RetroGaming.DAL.Repositories.Interfaces;

namespace RetroGaming.DAL.Repositories
{
    public class ConsoleRepository : IConsoleRepository
    {
        private readonly AppDbContext _context;

        public ConsoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GameConsole>> GetAllAsync(string? search = null)
        {
            var searchParam = new SqlParameter("@Search",
                string.IsNullOrWhiteSpace(search) ? DBNull.Value : (object)search);

            var results = await _context.Database
                .SqlQueryRaw<ConsoleResult>(
                    "EXEC sp_GetAllConsoles @Search", searchParam)
                .ToListAsync();

            return results.Select(MapToEntity).ToList();
        }

        public async Task<GameConsole?> GetByIdAsync(int id)
        {
            var param = new SqlParameter("@Id", id);

            var results = await _context.Database
                .SqlQueryRaw<ConsoleResult>(
                    "EXEC sp_GetConsoleById @Id", param)
                .ToListAsync();

            var result = results.FirstOrDefault();
            return result == null ? null : MapToEntity(result);
        }

        public async Task<bool> NameExistsForManufacturerAsync(
            int manufacturerId, string name, int? excludeId = null)
        {
            var result = await _context.Database
                .SqlQueryRaw<bool>(
                    "EXEC sp_ConsoleNameExistsForManufacturer @ManufacturerId, @Name, @ExcludeId",
                    new SqlParameter("@ManufacturerId", manufacturerId),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@ExcludeId", (object?)excludeId ?? DBNull.Value))
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<GameConsole> CreateAsync(GameConsole console)
        {
            var newId = await _context.Database
                .SqlQueryRaw<int>(
                    "EXEC sp_CreateConsole @ManufacturerId, @Name, @ReleaseYear, @Generation, @UnitsSoldMillions, @IsDiscontinued",
                    new SqlParameter("@ManufacturerId", console.ManufacturerId),
                    new SqlParameter("@Name", console.Name),
                    new SqlParameter("@ReleaseYear", console.ReleaseYear),
                    new SqlParameter("@Generation", console.Generation),
                    new SqlParameter("@UnitsSoldMillions", (object?)console.UnitsSoldMillions ?? DBNull.Value),
                    new SqlParameter("@IsDiscontinued", console.IsDiscontinued))
                .ToListAsync();

            console.Id = newId.FirstOrDefault();
            return console;
        }

        public async Task<GameConsole> UpdateAsync(GameConsole console)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateConsole @Id, @ManufacturerId, @Name, @ReleaseYear, @Generation, @UnitsSoldMillions, @IsDiscontinued",
                new SqlParameter("@Id", console.Id),
                new SqlParameter("@ManufacturerId", console.ManufacturerId),
                new SqlParameter("@Name", console.Name),
                new SqlParameter("@ReleaseYear", console.ReleaseYear),
                new SqlParameter("@Generation", console.Generation),
                new SqlParameter("@UnitsSoldMillions", (object?)console.UnitsSoldMillions ?? DBNull.Value),
                new SqlParameter("@IsDiscontinued", console.IsDiscontinued));

            return console;
        }

        public async Task DeleteAsync(GameConsole console)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_DeleteConsole @Id",
                new SqlParameter("@Id", console.Id));
        }

        private static GameConsole MapToEntity(ConsoleResult r) => new()
        {
            Id = r.Id,
            ManufacturerId = r.ManufacturerId,
            ManufacturerName = r.ManufacturerName,
            Name = r.Name,
            ReleaseYear = r.ReleaseYear,
            Generation = r.Generation,
            UnitsSoldMillions = r.UnitsSoldMillions,
            IsDiscontinued = r.IsDiscontinued,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}