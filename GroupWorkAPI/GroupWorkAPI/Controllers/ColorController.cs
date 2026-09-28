using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;
        [HttpGet("GetAllColor")]
        public async Task<IActionResult> GetAllColor()
        {
            List<Color> colors = await _context.Colors.ToListAsync();
            if(colors.Count == 0)
            {
                return NotFound("Цветов не найдено!");
            }
            List<ColorDTO> colorDTO = [];
            foreach(var color in colors)
            {
                colorDTO.Add(color.ToDto());
            }
            return Ok(colorDTO);
        }
        [HttpGet("GetColor/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            Color? color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);
            if (color == null)
            {
                return NotFound("Цвет не найдена!");
            }
            return Ok(color.ToDto()); 
        }
        [HttpPost("AddColor")]
        public async Task<IActionResult> AddColor([FromBody] ColorDTO colorDTO)
        {
            if (await _context.Colors.AnyAsync(x => x.Name == colorDTO.Name))
            {
                return BadRequest("Цвет с таким названием уже существует!");
            }
            int id = await _context.Colors.AnyAsync() ? await _context.Colors.MaxAsync(x => x.Id) + 1 : 1;
            Color color = new()
            {
                Id = id,
                Name = colorDTO.Name,
            };
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return Ok(color.ToDto());
        }
        [HttpPut("EditColor")]
        public async Task<IActionResult> EditColor([FromBody] ColorDTO colorDTO)
        {
            Color? color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == colorDTO.Id);
            if (color == null)
            {
                return NotFound("Цвет не найдена!");
            }
            if (await _context.Colors.AnyAsync(x => x.Name == colorDTO.Name))
            {
                return BadRequest("Цвет уже существует!");
            }
            color.Name = colorDTO.Name;
            _context.Colors.Update(color);
            await _context.SaveChangesAsync();
            return Ok(color.ToDto());
        }
    }
}
