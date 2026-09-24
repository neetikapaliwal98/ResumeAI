namespace ResumeAI.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresOn { get; set; }

    public DateTime? RevokedOn { get; set; }

    public ApplicationUser User { get; set; } = null!;
}