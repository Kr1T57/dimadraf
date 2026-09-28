using GroupWorkAPI.DataClasses;
using GroupWorkAPI.Internal;
using GroupWorkAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace GroupWorkAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(PostgresContext context) : ControllerBase
    {
        readonly PostgresContext _context = context;

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            List<Order> Order = await _context.Orders.ToListAsync();
            if (Order.Count == 0)
            {
                return NotFound("Заказов не найдено");
            }
            List<OrderDTO> OrderDTO = [];
            foreach (var order in Order)
            {
                OrderDTO.Add(order.ToDto());
            }
            return Ok(OrderDTO);
        }

        [HttpGet("GetOrder/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            Order? Orders = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (Orders == null)
            {
                return NotFound("Заказ не найден!");
            }
            return Ok(Orders.ToDto());
        }

        [HttpPut("EditStatusOrder")]
        public async Task<IActionResult> EditStatusOrder([FromBody] OrderDTO orderDTO)
        {
            Order? order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderDTO.Id);
            if (order == null)
            {
                return NotFound("Заказ не найден!");
            }
            OrderStatus? status = await _context.OrderStatuses.FirstOrDefaultAsync(x => x.Name == orderDTO.Status);
            if (status == null)
            {
                return NotFound("Статус не найден");
            }
            order.StatusId = status.Id;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok(order.ToDto());
        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderDTO orderDTO)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Email == orderDTO.User);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }
            OrderStatus? status = await _context.OrderStatuses.FirstOrDefaultAsync(x => x.Name == orderDTO.Status);
            if (status == null)
            {
                return NotFound("Статус не найден");
            }
            int id = await _context.Orders.AnyAsync() ? await _context.Orders.MaxAsync(x => x.Id) + 1 : 1;
            Order order = new()
            {
                Id = id,
                UserId = user.Id,
                OrderDate = DateTime.Now,
                StatusId = status.Id,
                TotalAmount = orderDTO.TotalAmount,
                City = orderDTO.City,
                Street = orderDTO.Street,
                House = orderDTO.House,
                PostalCode = orderDTO.PostalCode,
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order.ToDto());
        }
        [HttpPost("addOrderItem")]
        public async Task<IActionResult> AddOrderItem([FromForm]int userId,[FromForm]int orderId)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }
            List<CartItem> cart = await _context.CartItems.Where(x=>x.UserId==user.Id).ToListAsync();
            foreach(var  cartItem in cart)
            {
                ProductVariant? product = await _context.ProductVariants.Include(pv => pv.Product).FirstOrDefaultAsync(x => x.Id == cartItem.ProductVariantId);
                if (product == null)
                {
                    return NotFound("Вариация продукта не найден");
                }
                int idOrderItems = await _context.OrderItems.AnyAsync() ? await _context.OrderItems.MaxAsync(x => x.Id) + 1 : 1;
                OrderItem orderItem = new()
                {
                    Id = idOrderItems,
                    OrderId = orderId,
                    ProductVariantId = cartItem.ProductVariantId,
                    Quantity = cartItem.Quantity,
                    PriceAtPurchase = product.Product.BasePrice
                };
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();
            }
            _context.CartItems.RemoveRange(cart);
            await _context.SaveChangesAsync();
            Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                await SendEmail.SendReceipt(_context, order);
            }
            return Ok();
        }

        [HttpPut("EditAddresOrder")]
        public async Task<IActionResult> EditAddresOrder([FromBody] OrderDTO orderDTO)  
        {
            Order? order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderDTO.Id);
            if (order == null)
            {
                return NotFound("Заказ не найден!");
            }
            order.City = orderDTO.City;
            order.Street = orderDTO.Street;
            order.House = orderDTO.House;
            order.PostalCode = orderDTO.PostalCode;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok(order.ToDto());
        }


        [HttpGet("GetUserOrderHistory/{userId}")]
        public async Task<IActionResult> GetUserOrderHistory(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Status.Name == "Доставлен") 
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Color)
                .OrderByDescending(o => o.OrderDate) 
                .ToListAsync();

            var result = orders.Select(o => new UserOrderHistoryDTO
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate.ToShortDateString(),
                Status = o.Status.Name,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new UserOrderItemDTO
                {
                    ProductVariantId = oi.ProductVariantId,
                    ProductName = oi.ProductVariant.Product.Name,
                    Color = oi.ProductVariant.Color.Name,
                    Size = oi.ProductVariant.Size,
                    Quantity = oi.Quantity,
                    Price = oi.PriceAtPurchase
                }).ToList()
            }).ToList();

            return Ok(result);
        }
    }
}
