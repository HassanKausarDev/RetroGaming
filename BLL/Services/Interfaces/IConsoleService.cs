using RetroGaming.Common.DTOs;

namespace RetroGaming.BLL.Services.Interfaces
{
    public interface IConsoleService
    {
        Task<IEnumerable<ConsoleDto>> GetAllAsync(string? search = null);
        Task<ConsoleDto> GetByIdAsync(int id);
        Task<ConsoleDto> CreateAsync(CreateConsoleDto dto);
        Task<ConsoleDto> UpdateAsync(int id, UpdateConsoleDto dto);
        Task DeleteAsync(int id);
    }
}