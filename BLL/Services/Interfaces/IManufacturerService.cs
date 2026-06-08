using RetroGaming.Common.DTOs;

namespace RetroGaming.BLL.Services.Interfaces
{
    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerDto>> GetAllAsync();
        Task<ManufacturerDto> GetByIdAsync(int id);
        Task<ManufacturerDto> CreateAsync(CreateManufacturerDto dto);
        Task<ManufacturerDto> UpdateAsync(int id, UpdateManufacturerDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ManufacturerMapDto>> GetMapDataAsync();
    }
}