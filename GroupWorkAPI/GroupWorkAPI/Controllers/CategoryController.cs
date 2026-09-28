using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;
        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            if(categories.Count == 0)
            {
                return NotFound("Категорий не найдено");
            }
            List<CategoryDTO> categoriesDTO = [];
            foreach(var category in categories)
            {
                categoriesDTO.Add(category.ToDto());
            }
            return Ok(categoriesDTO);
        }
        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(x=> x.Id == id);
            if(category == null)
            {
                return NotFound("Категория не найдена!");
            }
            return Ok(category.ToDto());
        }   
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDTO)
        {
            if(await _context.Categories.AnyAsync(x=>x.Name == categoryDTO.Name))
            {
                return BadRequest("Категория с таким названием уже существует!");
            }
            int id = await _context.Categories.AnyAsync()? await _context.Categories.MaxAsync(x=>x.Id)+1:1;
            Category category = new()
            {
                Id = id,
                Name = categoryDTO.Name,
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category.ToDto());
        }
        [HttpPut("EditCategory")]
        public async Task<IActionResult> EditCategory([FromBody] CategoryDTO categoryDTO)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id == categoryDTO.Id);
            if(category == null)
            {
                return NotFound("Категория не найдена!");
            }
            if(await _context.Categories.AnyAsync(x=>x.Name==categoryDTO.Name))
            {
                return BadRequest("Категория уже существует!");
            }
            category.Name = categoryDTO.Name;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return Ok(category.ToDto());
        }
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id == id);
            if(category == null)
            {
                return NotFound("Категория не найдена");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
