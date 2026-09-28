using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;
        [HttpGet("GetAllCartItems")]
        public async Task<IActionResult> GetAllCartItems()
        {
            List<CartItem> cartitems = await _context.CartItems
                .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Product)
                .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Color)
                .ToListAsync();
            if (cartitems.Count == 0)
            {
                return NotFound("Данных в корзине не найдено");
            }
            List<CartItemDTO> cartItemDTO = [];
            foreach (var cart in cartitems)
            {
                cartItemDTO.Add(cart.ToDto());
            }
            return Ok(cartItemDTO);
        }
        [HttpGet("GetCartItem/{id}")]
        public async Task<IActionResult> GetCartItem(int id)
        {
            var cartitems = await _context.CartItems
                .Where(x => x.UserId == id) 
                .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Product)
                .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Color)
                .ToListAsync();
            List<CartItemDTO> cartItemDTOs = [];
            if (cartitems.Count == 0)
            {
                return Ok(new List<CartItemDTO>());
            }   
            foreach (var cart in cartitems)
            {
                cartItemDTOs.Add(cart.ToDto());
            }
            return Ok(cartItemDTOs);
        }

        [HttpGet("GetCartForSum/{id}")]
        public async Task<IActionResult> GetCartForSum(int id)
        {
            var cartitems = await _context.CartItems
                .FirstOrDefaultAsync(x => x.UserId == id);
            if (cartitems == null)
            {
                return NotFound("Не найдено");
            }
            return Ok(cartitems.ToDto());
        }

        [HttpPost("AddCartItem")]
        public async Task<IActionResult> AddCartItem([FromBody] CartItemDTO cartItemDTO)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == cartItemDTO.UserId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }
            ProductVariant? productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x => x.Id == cartItemDTO.ProductVariantId);
            if (productVariant == null)
            {
                return NotFound("Вариации продукта не найдено");
            }
            if (productVariant.StockQuantity == 0)
            {
                return BadRequest("Количество товара закончилось!");
            }
            CartItem? cart = await _context.CartItems.FirstOrDefaultAsync(x=>x.UserId == cartItemDTO.UserId && x.ProductVariantId == cartItemDTO.ProductVariantId);
            if (cart != null)
            {
                cart.Quantity += 1;
                _context.CartItems.Update(cart);
                //return Ok(cart.ToDto());
            }
            else
            {
                int id = await _context.CartItems.AnyAsync() ? await _context.CartItems.MaxAsync(x => x.Id) + 1 : 1;
                cart = new()
                {
                    Id = id,
                    UserId = user.Id,
                    ProductVariantId = productVariant.Id,
                    Quantity = 1
                };
                _context.CartItems.Add(cart);
            }
            productVariant.StockQuantity -= 1;
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
            return Ok(cart.ToDto());
        }
        [HttpPut("SubtractCartItem")]
        public async Task<IActionResult> SubtractCartItem([FromBody] CartItemDTO cartItemDTO)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == cartItemDTO.UserId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }
            ProductVariant? productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x => x.Id == cartItemDTO.ProductVariantId);
            if (productVariant == null)
            {
                return NotFound("Вариации продукта не найдено");
            }
            CartItem? cart = await _context.CartItems.FirstOrDefaultAsync(x => x.UserId == cartItemDTO.UserId && x.ProductVariantId == cartItemDTO.ProductVariantId);
            if (cart == null)
            {
                return BadRequest("Данный товар не находится в корзине");
            }
            cart.Quantity -= 1;
            if(cart.Quantity > 0)
            {
                _context.CartItems.Update(cart);
            }
            else
            {
                _context.CartItems.Remove(cart);
            }
            productVariant.StockQuantity += 1;
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
            return Ok(cart.ToDto());
        }
        [HttpDelete("DeleteCartItem/{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            CartItem? cartitem = await _context.CartItems.FirstOrDefaultAsync(x => x.Id == id);
            if (cartitem == null)
            {
                return NotFound("Элемент в корзине не найден");
            }
            ProductVariant? productVariant = await _context.ProductVariants
                        .FirstOrDefaultAsync(x => x.Id == cartitem.ProductVariantId);

            if (productVariant != null)
            {
                productVariant.StockQuantity += cartitem.Quantity;
                _context.ProductVariants.Update(productVariant);
            }
            else
            {
                return NotFound("Вариация продукта, привязанная к корзине, не найдена");
            }
            _context.CartItems.Remove(cartitem);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
