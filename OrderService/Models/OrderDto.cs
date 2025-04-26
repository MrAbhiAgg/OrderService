namespace OrderService.Models
{
    public class OrderDto
    {
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.MinValue;
    }

}
