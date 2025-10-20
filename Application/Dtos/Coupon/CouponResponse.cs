namespace General.Dto.Coupon
{
    public class CouponResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal Discount { get; set; }
        public bool Active { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
