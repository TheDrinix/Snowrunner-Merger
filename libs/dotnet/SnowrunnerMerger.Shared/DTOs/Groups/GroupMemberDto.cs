namespace SnowrunnerMerger.Shared.DTOs.Groups;

public record GroupMemberDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
};