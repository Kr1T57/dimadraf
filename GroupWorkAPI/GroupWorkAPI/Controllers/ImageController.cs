using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Internal;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;
        [HttpGet("GetImageAll")]
        public async Task<IActionResult> GetImageAll()
        {
            List<ProductImage> productImages = await _context.ProductImages.ToListAsync();
            if (productImages.Count == 0)
            {
                return NotFound("Фотографий не найдено");
            }
            List<ProductImageDTO> productImagesDTO = [];
            foreach (var productImage in productImages)
            {
                productImagesDTO.Add(productImage.ToDto());
            }
            return Ok(productImagesDTO);
        }
        [HttpGet("GetProductImage/{id}")]
        public async Task<IActionResult> GetProductImage(int id)
        {
            ProductImage? productImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.ProductId == id);
            if (productImage == null)
            {
                return NotFound("Фотографий не найдено!");
            }
            return Ok(productImage.ToDto());
        }
        [HttpGet("GetVariantImage/{id}")]
        public async Task<IActionResult> GetVariantImage(int id)
        {
            var variantImage = await _context.VariantImages.FirstOrDefaultAsync(x => x.ProductVariantId == id);
            if (variantImage == null) return NotFound("Фото вариации не найдено");
            return Ok(variantImage.ToDto()); 
        }

        [HttpGet("GetProductImageByVariant/{variantId}")]
        public async Task<IActionResult> GetProductImageByVariant(int variantId)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId);

            if (variant == null)
            {
                return NotFound("Вариация товара не найдена!");
            }

            ProductImage? productImage = await _context.ProductImages
                .FirstOrDefaultAsync(x => x.ProductId == variant.ProductId);

            if (productImage == null)
            {
                return NotFound("Фотография для этого товара не найдена!");
            }

            return Ok(productImage.ToDto());
        }

        [HttpPost("UpLoadProductImage")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UpLoadProductImage([FromForm] UploadedFile uploadedFile, [FromForm] ProductDTO productDto)
        {
            if (uploadedFile.File == null || uploadedFile.File.Length == 0)
            {
                return BadRequest("Файл не передан или пустой");
            }
            string path = Path.Combine("wwwroot", "images");
            string ext = Path.GetExtension(uploadedFile.File.FileName);
            string fileName = $"{productDto.Id}-{productDto.Category}-{productDto.Name}{ext}";
            path = Path.Combine(path, fileName);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await uploadedFile.File.CopyToAsync(stream);
            }
            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productDto.Id);
            if (product == null)
            {
                return NotFound("Товар не найден");
            }
            int id = await _context.ProductImages.AnyAsync() ? await _context.ProductImages.MaxAsync(x => x.Id) + 1 : 1;
            ProductImage productImage = new()
            {
                Id = id,
                ProductId = product.Id,
                ImageUrl = fileName,
                IsMain = true,
            };
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();
            return Ok(productImage.ToDto());
        }
        [HttpDelete("DeleteProductImage/{id}")]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            ProductImage? productImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id == id);
            if (productImage == null)
            {
                return NotFound("Фотографий не найдено!");
            }
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("UpLoadProductVariantImage")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UpLoadProductVariantImage([FromForm] UploadedFile uploadedFile, [FromForm] ProductVariantDTO variant)
        {
            if (uploadedFile.File == null || uploadedFile.File.Length == 0)
            {
                return BadRequest("Файл не передан или пустой");
            }
            ProductVariant? productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x=>x.Id==variant.Id);
            if(productVariant == null)
            {
                return NotFound("Вариации продукта не найдена");
            }
            string path = Path.Combine("wwwroot", "imagesVariant");
            string ext = Path.GetExtension(uploadedFile.File.FileName);
            string fileName = $"{variant.Id}-{variant.Color}-{variant.Size}{ext}";
            path = Path.Combine(path, fileName);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await uploadedFile.File.CopyToAsync(stream);
            }
            int id = await _context.VariantImages.AnyAsync() ? await _context.VariantImages.MaxAsync(x => x.Id) + 1 : 1;
            VariantImage variantImage = new()
            {
                Id = id,
                ProductVariantId = productVariant.Id,
                ImagePath = fileName,
            };
            _context.VariantImages.Add(variantImage);
            await _context.SaveChangesAsync();
            return Ok(variantImage.ToDto());
        }
    }
}
