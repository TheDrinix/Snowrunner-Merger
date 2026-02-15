using System.ComponentModel.DataAnnotations;

namespace SnowrunnerMerger.Api.Models.Saves.Dtos;

public record CreateGroupDto
{
    [Microsoft.Build.Framework.Required]
    [MinLength(3)]
    [MaxLength(64)]
    public string Name { get; set; }
};