using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<OrderController> _logger;
        private readonly KafkaProducer _kafka;
        public OrderController(OrderDbContext context, ILogger<OrderController> logger, KafkaProducer kafka)
        {
            _context = context;
            _logger = logger;
            _kafka = kafka;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderDto orderDto)
        {
            var order = new Order
            {
                ProductName = orderDto.ProductName,
                CustomerName = orderDto.CustomerName,
                Status = "Pending",
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Order {order.Id} created successfully.");
            await _kafka.SendAsync($"New Order Created: {order.Id}");

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                _logger.LogWarning($"Order {id} not found.");
                return NotFound();
            }

            return order;
        }
    }

}
