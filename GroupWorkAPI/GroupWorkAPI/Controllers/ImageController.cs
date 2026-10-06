using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Internal;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
        [RequestSizeLimit(15_000_000)]
        public async Task<IActionResult> UpLoadProductImage([FromForm] IFormFile? file, [FromForm] int productId, [FromForm] string? category, [FromForm] string? name)
        {
            // Поддерживаем как прямую передачу, так и вложенный объект File
            var formFile = file ?? Request.Form.Files["File"] ?? Request.Form.Files["file"];
            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest("Файл не передан или пустой");
            }

            int pId = productId;
            if (pId == 0 && int.TryParse(Request.Form["productDto.Id"], out int parsedId)) pId = parsedId;
            if (pId == 0 && int.TryParse(Request.Form["productDto[id]"], out int parsedId2)) pId = parsedId2;

            string prodCategory = category ?? Request.Form["productDto.Category"].ToString() ?? Request.Form["productDto[category]"].ToString() ?? "Product";
            string prodName = name ?? Request.Form["productDto.Name"].ToString() ?? Request.Form["productDto[name]"].ToString() ?? "Item";

            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == pId);
            if (product == null)
            {
                return NotFound($"Товар с ID {pId} не найден");
            }

            // ВАЖНО: Гарантируем, что папка существует на диске (защита от 500 ошибки)
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string ext = Path.GetExtension(formFile.FileName);
            string safeName = Regex.Replace(prodName, @"[^\w\.-]", "_");
            string fileName = $"{pId}-{prodCategory}-{safeName}{ext}";
            string fullPath = Path.Combine(dirPath, fileName);

            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            int imgId = await _context.ProductImages.AnyAsync() ? await _context.ProductImages.MaxAsync(x => x.Id) + 1 : 1;
            ProductImage productImage = new()
            {
                Id = imgId,
                ProductId = product.Id,
                ImageUrl = fileName,
                IsMain = true,
            };

            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();
            return Ok(productImage.ToDto());
        }

        [HttpPost("UpLoadProductVariantImage")]
        [RequestSizeLimit(15_000_000)]
        public async Task<IActionResult> UpLoadProductVariantImage([FromForm] IFormFile? file, [FromForm] int variantId, [FromForm] string? color, [FromForm] string? size)
        {
            var formFile = file ?? Request.Form.Files["File"] ?? Request.Form.Files["file"];
            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest("Файл не передан или пустой");
            }

            int vId = variantId;
            if (vId == 0 && int.TryParse(Request.Form["variant.Id"], out int pId)) vId = pId;
            if (vId == 0 && int.TryParse(Request.Form["variant[id]"], out int pId2)) vId = pId2;

            ProductVariant? productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x => x.Id == vId);
            if (productVariant == null)
            {
                return NotFound("Вариация продукта не найдена");
            }

            // ВАЖНО: Создаем папку imagesVariant (защита от 500 ошибки)
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesVariant");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string vColor = color ?? Request.Form["variant.Color"].ToString() ?? "Color";
            string vSize = size ?? Request.Form["variant.Size"].ToString() ?? "Size";

            string ext = Path.GetExtension(formFile.FileName);
            string fileName = $"{vId}-{vColor}-{vSize}{ext}";
            string fullPath = Path.Combine(dirPath, fileName);

            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
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
    }
}
