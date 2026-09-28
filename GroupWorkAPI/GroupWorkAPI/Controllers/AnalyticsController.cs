using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly PostgresContext _context;

        public AnalyticsController(PostgresContext context)
        {
            _context = context;
        }

        [HttpGet("GetMostLikedProducts")]
        public async Task<IActionResult> GetMostLikedProducts()
        {
            var data = await _context.ProductFavorites
                .GroupBy(f => f.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = _context.Products
                        .Where(p => p.Id == g.Key)
                        .Select(p => p.Name)
                        .FirstOrDefault() ?? "Неизвестный товар",
                    LikesCount = g.Count()
                })
                .OrderByDescending(x => x.LikesCount)
                .Take(10) 
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("GetMostLikedVariants")]
        public async Task<IActionResult> GetMostLikedVariants()
        {
            var data = await _context.VariantFavorites
                .GroupBy(f => f.ProductVariantId)
                .Select(g => new
                {
                    ProductVariantId = g.Key,
                    VariantName = _context.ProductVariants
                        .Where(v => v.Id == g.Key)
                        .Select(v => v.Product.Name + " (Цвет: " + v.Color.Name + ", Размер: " + v.Size + ")")
                        .FirstOrDefault() ?? "Неизвестная вариация",
                    LikesCount = g.Count()
                })
                .OrderByDescending(x => x.LikesCount)
                .Take(10)
                .ToListAsync();

            return Ok(data);
        }
        [HttpPost("ToggleProductFavorite")]
        public async Task<IActionResult> ToggleProductFavorite([FromBody] ProductFavoriteDTO dto)
        {
            var existing = await _context.ProductFavorites
                .FirstOrDefaultAsync(f => f.UserId == dto.UserId && f.ProductId == dto.ProductId);

            if (existing != null)
            {
                _context.ProductFavorites.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { isLiked = false });
            }
            else
            {
                int id = await _context.ProductFavorites.AnyAsync()
                    ? await _context.ProductFavorites.MaxAsync(x => x.Id) + 1
                    : 1;

                ProductFavorite newFavorite = new()
                {
                    Id = id,
                    UserId = dto.UserId,
                    ProductId = dto.ProductId
                };

                _context.ProductFavorites.Add(newFavorite);
                await _context.SaveChangesAsync();
                return Ok(new { isLiked = true });
            }
        }

        [HttpPost("ToggleVariantFavorite")]
        public async Task<IActionResult> ToggleVariantFavorite([FromBody] VariantFavoriteDTO dto)
        {
            var existing = await _context.VariantFavorites
                .FirstOrDefaultAsync(f => f.UserId == dto.UserId && f.ProductVariantId == dto.ProductVariantId);

            if (existing != null)
            {
                _context.VariantFavorites.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { isLiked = false });
            }
            else
            {
                int id = await _context.VariantFavorites.AnyAsync()
                    ? await _context.VariantFavorites.MaxAsync(x => x.Id) + 1
                    : 1;

                VariantFavorite newFavorite = new()
                {
                    Id = id,
                    UserId = dto.UserId,
                    ProductVariantId = dto.ProductVariantId
                };

                _context.VariantFavorites.Add(newFavorite);
                await _context.SaveChangesAsync();
                return Ok(new { isLiked = true });
            }
        }

        [HttpGet("GetUserProductFavorites/{userId}")]
        public async Task<IActionResult> GetUserProductFavorites(int userId)
        {
            var productIds = await _context.ProductFavorites
                .Where(f => f.UserId == userId)
                .Select(f => f.ProductId)
                .ToListAsync();

            return Ok(productIds);
        }

        [HttpGet("GetUserVariantFavorites/{userId}")]
        public async Task<IActionResult> GetUserVariantFavorites(int userId)
        {
            var variantIds = await _context.VariantFavorites
                .Where(f => f.UserId == userId)
                .Select(f => f.ProductVariantId)
                .ToListAsync();

            return Ok(variantIds);
        }
        [HttpGet("GetActualUserProductFavorites/{userId}")]
        public async Task<IActionResult> GetActualUserProductFavorites(int userId)
        {
            var data = await _context.ProductFavorites
                .Where(f => f.UserId == userId)
                .Select(f => new
                {
                    ProductId = f.ProductId,
                    ProductName = _context.Products
                        .Where(p => p.Id == f.ProductId)
                        .Select(p => p.Name)
                        .FirstOrDefault() ?? "Неизвестный товар"
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("GetActualUserVariantFavorites/{userId}")]
        public async Task<IActionResult> GetActualUserVariantFavorites(int userId)
        {
            var data = await _context.VariantFavorites
                .Where(f => f.UserId == userId)
                .Select(f => new
                {
                    ProductVariantId = f.ProductVariantId,
                    ProductId = _context.ProductVariants
                        .Where(v => v.Id == f.ProductVariantId)
                        .Select(v => v.ProductId)
                        .FirstOrDefault(),
                    VariantName = _context.ProductVariants
                        .Where(v => v.Id == f.ProductVariantId)
                        .Select(v => v.Product.Name)
                        .FirstOrDefault() ?? "Неизвестная модель",
                    Color = _context.ProductVariants
                        .Where(v => v.Id == f.ProductVariantId)
                        .Select(v => v.Color.Name)
                        .FirstOrDefault() ?? "Не указан",
                    Size = _context.ProductVariants
                        .Where(v => v.Id == f.ProductVariantId)
                        .Select(v => v.Size)
                        .FirstOrDefault() 
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
