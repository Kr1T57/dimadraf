using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;
        [HttpGet("GetAllBrands")]
        public async Task<IActionResult> GetAllBrands()
        {
            List<Brand> brands = await _context.Brands.ToListAsync();
            if(brands.Count == 0)
            {
                return NotFound("Бренды не найдены!");
            }
            List<BrandDTO> brandDTO = [];
            foreach(var  brand in brands)
            {
                brandDTO.Add(brand.ToDto());
            }
            return Ok(brandDTO);
        }
        [HttpGet("GetBrand/{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(x=>x.Id == id);
            if(brand == null)
            {
                return NotFound("Бренд не найден!");
            }
            return Ok(brand.ToDto());
        }
        [HttpGet("GetProductBrand")]
        public async Task<IActionResult> GetProductBrand(int id)
        {
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(x=>x.Id == id);
            if(brand == null)
            {
                return NotFound("Бренд не найден!");
            }
            List<Product> products = await _context.Products.Include(x=>x.ProductImages).ToListAsync();
            products = [..products.Where(x=>x.BrandId==id)];
            if(products.Count == 0)
            {
                return NotFound("Товаров с данным брендом не найдено!");
            }
            List<ProductDTO> productDTO = [];
            foreach(var product in products)
            {
                productDTO.Add(product.ToDto());
            }
            return Ok(productDTO);
        }
        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand([FromBody] BrandDTO brandDTO)
        {
            if(await _context.Brands.AnyAsync(x=>x.Name == brandDTO.Name))
            {
                return BadRequest("Бренд с такимм названием уже существует!");
            }
            int id = await _context.Brands.AnyAsync() ? await _context.Brands.MaxAsync(x => x.Id) + 1 : 1;
            Brand brand = new()
            {
                Id = id,
                Name = brandDTO.Name,
            };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return Ok(brand.ToDto());
        }
        [HttpPut("EditBrand")]
        public async Task<IActionResult> EditBrand([FromBody] BrandDTO brandDTO)
        { 
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(x=>x.Id == brandDTO.Id);
            if (brand == null)
            {
                return NotFound("Бренд не найден!");
            }
            if(await _context.Brands.AnyAsync(x => x.Name == brandDTO.Name))
            {
                return BadRequest("Бренд уже существует!");
            }
            brand.Name = brandDTO.Name;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return Ok(brand.ToDto());
        }
        [HttpDelete("DeleteBrand")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(x=>x.Id==id);
            if (brand == null)
            {
                return NotFound("Бренд не найден");
            }
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
