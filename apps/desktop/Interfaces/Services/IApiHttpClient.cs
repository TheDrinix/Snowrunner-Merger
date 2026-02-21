using System.Collections.Generic;
using System.Threading.Tasks;
using SnowrunnerMerger.Shared.DTOs.Groups;

namespace SnowrunnerMerger.Desktop.Interfaces.Services;

public interface IApiHttpClient
{
    Task<IEnumerable<GroupDto>> GetGroupsAsync();
}