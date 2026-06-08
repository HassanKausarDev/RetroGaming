using RetroGaming.DAL.Entities;

namespace RetroGaming.DAL.Repositories.Interfaces
{
    public interface IManufacturerRepository
    {
        Task<IEnumerable<Manufacturer>> GetAllAsync();
        Task<Manufacturer?> GetByIdAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task<bool> HasConsolesAsync(int id);
        Task<Manufacturer> CreateAsync(Manufacturer manufacturer);
        Task<Manufacturer> UpdateAsync(Manufacturer manufacturer);
        Task DeleteAsync(Manufacturer manufacturer);
    }
}