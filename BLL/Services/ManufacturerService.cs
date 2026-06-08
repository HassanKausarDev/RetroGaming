using AutoMapper;
using RetroGaming.BLL.Services.Interfaces;
using RetroGaming.Common.DTOs;
using RetroGaming.Common.Exceptions;
using RetroGaming.DAL.Entities;
using RetroGaming.DAL.Repositories.Interfaces;

namespace RetroGaming.BLL.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IManufacturerRepository _repository;
        private readonly IMapper _mapper;

        public ManufacturerService(IManufacturerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ManufacturerDto>> GetAllAsync()
        {
            var manufacturers = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ManufacturerDto>>(manufacturers);
        }

        public async Task<ManufacturerDto> GetByIdAsync(int id)
        {
            var manufacturer = await _repository.GetByIdAsync(id)
                ?? throw AppException.NotFound("Manufacturer", id);

            return _mapper.Map<ManufacturerDto>(manufacturer);
        }

        public async Task<ManufacturerDto> CreateAsync(CreateManufacturerDto dto)
        {
            if (await _repository.NameExistsAsync(dto.Name))
                throw AppException.Conflict(
                    $"A manufacturer named '{dto.Name}' already exists.");

            if (dto.FoundedYear > DateTime.UtcNow.Year)
                throw AppException.BadRequest(
                    $"Founded year ({dto.FoundedYear}) cannot be in the future.");

            var entity = _mapper.Map<Manufacturer>(dto);
            var created = await _repository.CreateAsync(entity);
            var result = await _repository.GetByIdAsync(created.Id);
            return _mapper.Map<ManufacturerDto>(result);
        }

        public async Task<ManufacturerDto> UpdateAsync(int id, UpdateManufacturerDto dto)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw AppException.NotFound("Manufacturer", id);

            if (await _repository.NameExistsAsync(dto.Name, excludeId: id))
                throw AppException.Conflict(
                    $"A manufacturer named '{dto.Name}' already exists.");

            if (dto.FoundedYear > DateTime.UtcNow.Year)
                throw AppException.BadRequest(
                    $"Founded year ({dto.FoundedYear}) cannot be in the future.");

            _mapper.Map(dto, entity);
            var updated = await _repository.UpdateAsync(entity);
            var result = await _repository.GetByIdAsync(updated.Id);
            return _mapper.Map<ManufacturerDto>(result);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw AppException.NotFound("Manufacturer", id);

            if (await _repository.HasConsolesAsync(id))
                throw AppException.BadRequest(
                    $"Cannot delete '{entity.Name}' because it still has consoles. " +
                    "Please delete or reassign those consoles first.");

            await _repository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<ManufacturerMapDto>> GetMapDataAsync()
        {
            var all = await _repository.GetAllAsync();
            var withCoords = all.Where(m => m.Latitude.HasValue && m.Longitude.HasValue);
            return _mapper.Map<IEnumerable<ManufacturerMapDto>>(withCoords);
        }
    }
}