namespace General.Dto.ApprovalJob
{
    public class ApprovalJobResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Operation { get; set; } = null!;
        public bool State { get; set; }                    
        public int RequestedBy { get; set; }
        public int? DecidedBy { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? DecidedAt { get; set; }
        public int? TargetId { get; set; }
        public string? Reason { get; set; }
    }
}
