using Microsoft.AspNetCore.Mvc;
using RetroGaming.BLL.Services.Interfaces;
using RetroGaming.Common.DTOs;

namespace RetroGaming.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _service;

        public ManufacturersController(IManufacturerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManufacturerDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ManufacturerDto>> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpGet("map")]
        public async Task<ActionResult<IEnumerable<ManufacturerMapDto>>> GetMapData()
            => Ok(await _service.GetMapDataAsync());

        [HttpPost]
        public async Task<ActionResult<ManufacturerDto>> Create(CreateManufacturerDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ManufacturerDto>> Update(int id, UpdateManufacturerDto dto)
            => Ok(await _service.UpdateAsync(id, dto));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}