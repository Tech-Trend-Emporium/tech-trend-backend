using Domain.Enums;

namespace Application.Dtos.Cart
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string? Address { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }

        public decimal? TotalAmount { get; set; }
        public DateTime? PlacedAtUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }

        public IReadOnlyList<OrderItemResponse> Items { get; set; } = Array.Empty<OrderItemResponse>();

    }
}
