using System;
using SnowrunnerMerger.Shared.DTOs.Groups;

namespace SnowrunnerMerger.Desktop.Models.Groups;

public class Group(GroupDto dto)
{
    public Guid Id { get; } = dto.Id;
    public string Name { get; } = dto.Name;
    public string InviteCode { get; } = dto.InviteCode;
    public Guid OwnerId { get; } = dto.OwnerId;
}