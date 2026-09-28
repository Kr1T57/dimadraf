using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductvariantController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;
        [HttpGet("GetProductVariantsAll")]
        public async Task<IActionResult> GetProductVariantsAll()
        {
            List<ProductVariant> productVariants = await _context.ProductVariants.ToListAsync();
            if (productVariants.Count == 0)
            {
                return NotFound("Вариаций продуктов не найдено");
            }
            List<ProductVariantDTO> productVariantDTO = [];
            foreach (var productVariant in productVariants)
            {
                productVariantDTO.Add(productVariant.ToDto());
            }
            return Ok(productVariantDTO);
        }
        [HttpGet("GetProductVariant/{id}")]
        public async Task<IActionResult> GetProductVariant(int id)
        {
            ProductVariant? productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x => x.Id == id);
            if (productVariant == null)
            {
                return NotFound("Вариация продукта не найдена!");
            }
            return Ok(productVariant.ToDto());
        }
        [HttpPost("AddProductVariant")]
        public async Task<IActionResult> AddProduct([FromBody] ProductVariantDTO productVariantDTO)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(x=>x.Id == productVariantDTO.ProductId);
            if (product == null)
            {
                return NotFound("Товар не найден");
            }
            Color? color = await _context.Colors.FirstOrDefaultAsync(x=>x.Name == productVariantDTO.Color);
            if (color == null)
            {
                return NotFound("Цвет не найден");
            }
            int id = await _context.ProductVariants.AnyAsync() ? await _context.ProductVariants.MaxAsync(x => x.Id) + 1 : 1;
            ProductVariant prouductVariant = new()
            {
                Id = id,
                ProductId = product.Id,
                ColorId = color.Id,
                Size = productVariantDTO.Size,
                StockQuantity = productVariantDTO.StockQuantity,
            };
            _context.ProductVariants.Add(prouductVariant);
            await _context.SaveChangesAsync();
            return Ok(prouductVariant.ToDto());
        }
        [HttpPut("EditProductVariant")]
        public async Task<IActionResult> EditProductVariant([FromBody] ProductVariantDTO productVariantDTO)
        {
            ProductVariant? productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x => x.Id == productVariantDTO.Id);
            if (productVariant == null)
            {
                return NotFound("Товар не найден!");
            }
            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productVariantDTO.ProductId);
            if (product == null)
            {
                return NotFound("Товар не найден");
            }
            Color? color = await _context.Colors.FirstOrDefaultAsync(x => x.Name == productVariantDTO.Color);
            if (color == null)
            {
                return NotFound("Цвет не найден");
            }
            if (productVariant.StockQuantity < 0) return BadRequest("Количество равно нулю!");
            productVariant.ProductId = product.Id;
            productVariant.ColorId = color.Id;
            productVariant.Size = productVariantDTO.Size;
            productVariant.StockQuantity = productVariantDTO.StockQuantity;
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
            return Ok(productVariant.ToDto());
        }
        [HttpDelete("DeleteProductVariant/{id}")]
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            ProductVariant? productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x => x.Id == id);
            if (productVariant == null)
            {
                return NotFound("Вариация продукта не найдено");
            }
            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
