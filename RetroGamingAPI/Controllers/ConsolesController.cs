using Microsoft.AspNetCore.Mvc;
using RetroGaming.BLL.Services.Interfaces;
using RetroGaming.Common.DTOs;

namespace RetroGaming.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsolesController : ControllerBase
    {
        private readonly IConsoleService _service;

        public ConsolesController(IConsoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsoleDto>>> GetAll(
            [FromQuery] string? search = null)
            => Ok(await _service.GetAllAsync(search));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConsoleDto>> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<ConsoleDto>> Create(CreateConsoleDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ConsoleDto>> Update(int id, UpdateConsoleDto dto)
            => Ok(await _service.UpdateAsync(id, dto));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}