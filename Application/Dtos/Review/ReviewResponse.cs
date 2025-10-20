namespace General.Dto.Review
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public int ProductId { get; set; }
        public string Comment { get; set; } = null!;
        public float Rating { get; set; }
    }
}
