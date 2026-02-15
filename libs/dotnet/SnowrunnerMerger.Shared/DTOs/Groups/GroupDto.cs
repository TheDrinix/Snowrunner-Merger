namespace SnowrunnerMerger.Shared.DTOs.Groups;

public record GroupDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string InviteCode { get; set; } = string.Empty;
  public ICollection<GroupMemberDto> Members { get; set; } = [];
  public Guid OwnerId { get; set; }
  public GroupMemberDto Owner { get; set; } = new GroupMemberDto();
};