using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Internal;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;
        [HttpGet("GetProductAll")]
        public async Task<IActionResult> GetProductAll()
        {
            List<Product> products = await _context.Products.ToListAsync();
            if(products.Count == 0)
            {
                return NotFound("Продуктов не найдено");
            }
            List<ProductDTO> productDTO = [];
            foreach (var product in products)
            {
                productDTO.Add(product.ToDto());
            }
            return Ok(productDTO);
        }

        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            Product? product = await _context.Products
                .Include(x=>x.ProductVariants)
                    .ThenInclude(v=>v.Color)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound("Продукт не найдена!");
            }
            return Ok(product.ToDto());
        }

        [HttpGet("GetProductCategory/{id}")]
        public async Task<IActionResult> GetProductCategory(int id)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound("Категория не найдена!");
            }
            List<Product> products = await _context.Products.Include(x => x.ProductImages).ToListAsync();
            products = [.. products.Where(x => x.CategoryId == id)];
            if (products.Count == 0)
            {
                return NotFound("Товаров с данной категорией не найдено!");
            }
            List<ProductDTO> productDTO = [];
            foreach (var product in products)
            {
                productDTO.Add(product.ToDto());
            }
            return Ok(productDTO);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDTO)
        {
            if (await _context.Products.AnyAsync(x => x.Name == productDTO.Name))
            {
                return BadRequest("Товар с таким названием уже существует!");
            }
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(x => x.Name == productDTO.Brand);
            if(brand == null)
            {
                return NotFound("Бренд не найден");
            }
            Category? category = await _context.Categories.FirstOrDefaultAsync(x=>x.Name == productDTO.Category);
            if (category == null)
            {
                return NotFound("Категория не найден");
            }
            int id = await _context.Products.AnyAsync() ? await _context.Products.MaxAsync(x => x.Id) + 1 : 1;
            Product prouduct = new()
            {
                Id = id,
                BrandId = brand.Id,
                CategoryId = category.Id,
                Name = productDTO.Name,
                Description = productDTO.Description,
                BasePrice = productDTO.BasePrice,
            };
            _context.Products.Add(prouduct);
            await _context.SaveChangesAsync();
            return Ok(prouduct.ToDto());
        }

        [HttpPut("EditProduct")]
        public async Task<IActionResult> EditProduct([FromBody] ProductDTO productDTO)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productDTO.Id);
            if (product == null)
            {
                return NotFound("Товар не найден!");
            }
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(x => x.Name == productDTO.Brand);
            if (brand == null)
            {
                return NotFound("Бренд не найден");
            }
            Category? category = await _context.Categories.FirstOrDefaultAsync(x => x.Name == productDTO.Category);
            if (category == null)
            {
                return NotFound("Категория не найден");
            }
            product.Name = productDTO.Name;
            product.BrandId = brand.Id;
            product.CategoryId = category.Id;
            product.Description = productDTO.Description;
            product.BasePrice = productDTO.BasePrice;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok(product.ToDto());
        }
        [HttpPut("EditProductVariant")]
        public async Task<IActionResult> EditProductVariant([FromBody] ProductVariantDTO variantDTO)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == variantDTO.Id);

            if (variant == null)
            {
                return NotFound("Вариант товара не найден");
            }

            var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == variantDTO.Color);
            if (color == null)
            {
                return NotFound("Указанный цвет не найден в базе");
            }

            variant.ColorId = color.Id;
            variant.Size = variantDTO.Size;
            variant.StockQuantity = variantDTO.StockQuantity;

            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();

            return Ok(variant.ToDto());
        }
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound("Продукт не найден");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
