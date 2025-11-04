namespace Application.Dtos.Auth
{
    public class VerifyRecoveryAnswerResponse
    {
        public string ResetToken { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
