using AutoMapper;
using RetroGaming.BLL.Services.Interfaces;
using RetroGaming.Common.DTOs;
using RetroGaming.Common.Exceptions;
using RetroGaming.DAL.Entities;
using RetroGaming.DAL.Repositories.Interfaces;

namespace RetroGaming.BLL.Services
{
    public class ConsoleService : IConsoleService
    {
        private readonly IConsoleRepository _consoleRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;

        public ConsoleService(
            IConsoleRepository consoleRepository,
            IManufacturerRepository manufacturerRepository,
            IMapper mapper)
        {
            _consoleRepository = consoleRepository;
            _manufacturerRepository = manufacturerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ConsoleDto>> GetAllAsync(string? search = null)
        {
            var consoles = await _consoleRepository.GetAllAsync(search);
            return _mapper.Map<IEnumerable<ConsoleDto>>(consoles);
        }

        public async Task<ConsoleDto> GetByIdAsync(int id)
        {
            var console = await _consoleRepository.GetByIdAsync(id)
                ?? throw AppException.NotFound("Console", id);

            return _mapper.Map<ConsoleDto>(console);
        }

        public async Task<ConsoleDto> CreateAsync(CreateConsoleDto dto)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(dto.ManufacturerId)
                ?? throw AppException.NotFound("Manufacturer", dto.ManufacturerId);

            if (dto.ReleaseYear < manufacturer.FoundedYear)
                throw AppException.BadRequest(
                    $"Release year ({dto.ReleaseYear}) cannot be before " +
                    $"{manufacturer.Name}'s founding year ({manufacturer.FoundedYear}).");

            if (dto.ReleaseYear > DateTime.UtcNow.Year)
                throw AppException.BadRequest(
                    $"Release year ({dto.ReleaseYear}) cannot be in the future.");

            if (await _consoleRepository.NameExistsForManufacturerAsync(
                    dto.ManufacturerId, dto.Name))
                throw AppException.Conflict(
                    $"'{manufacturer.Name}' already has a console named '{dto.Name}'.");

            var entity = _mapper.Map<GameConsole>(dto);
            var created = await _consoleRepository.CreateAsync(entity);
            var result = await _consoleRepository.GetByIdAsync(created.Id);
            return _mapper.Map<ConsoleDto>(result);
        }

        public async Task<ConsoleDto> UpdateAsync(int id, UpdateConsoleDto dto)
        {
            var console = await _consoleRepository.GetByIdAsync(id)
                ?? throw AppException.NotFound("Console", id);

            var manufacturer = await _manufacturerRepository.GetByIdAsync(dto.ManufacturerId)
                ?? throw AppException.NotFound("Manufacturer", dto.ManufacturerId);

            if (dto.ReleaseYear < manufacturer.FoundedYear)
                throw AppException.BadRequest(
                    $"Release year ({dto.ReleaseYear}) cannot be before " +
                    $"{manufacturer.Name}'s founding year ({manufacturer.FoundedYear}).");

            if (dto.ReleaseYear > DateTime.UtcNow.Year)
                throw AppException.BadRequest(
                    $"Release year ({dto.ReleaseYear}) cannot be in the future.");

            if (await _consoleRepository.NameExistsForManufacturerAsync(
                    dto.ManufacturerId, dto.Name, excludeId: id))
                throw AppException.Conflict(
                    $"'{manufacturer.Name}' already has a console named '{dto.Name}'.");

            _mapper.Map(dto, console);
            var updated = await _consoleRepository.UpdateAsync(console);
            var result = await _consoleRepository.GetByIdAsync(updated.Id);
            return _mapper.Map<ConsoleDto>(result);
        }

        public async Task DeleteAsync(int id)
        {
            var console = await _consoleRepository.GetByIdAsync(id)
                ?? throw AppException.NotFound("Console", id);

            await _consoleRepository.DeleteAsync(console);
        }
    }
}