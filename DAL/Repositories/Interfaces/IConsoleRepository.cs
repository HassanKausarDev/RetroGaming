using RetroGaming.DAL.Entities;

namespace RetroGaming.DAL.Repositories.Interfaces
{
    public interface IConsoleRepository
    {
        Task<IEnumerable<GameConsole>> GetAllAsync(string? search = null);
        Task<GameConsole?> GetByIdAsync(int id);
        Task<bool> NameExistsForManufacturerAsync(
                                           int manufacturerId,
                                           string name,
                                           int? excludeId = null);
        Task<GameConsole> CreateAsync(GameConsole console);
        Task<GameConsole> UpdateAsync(GameConsole console);
        Task DeleteAsync(GameConsole console);
    }
}