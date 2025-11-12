namespace Application.Dtos.Cart
{
    public class OrderItemResponse
    {
        public int ProductId { get; set; }
        public string Title { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
