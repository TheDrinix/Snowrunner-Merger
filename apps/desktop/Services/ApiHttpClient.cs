using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SnowrunnerMerger.Desktop.Interfaces.Services;
using SnowrunnerMerger.Shared.DTOs.Groups;

namespace SnowrunnerMerger.Desktop.Services;

public class ApiHttpClient(HttpClient httpClient) : IApiHttpClient
{
    public async Task<IEnumerable<GroupDto>> GetGroupsAsync()
    {
        var response = await httpClient.GetAsync("groups");
        
        response.EnsureSuccessStatusCode();

        var groups = await response.Content.ReadFromJsonAsync<IEnumerable<GroupDto>>();

        return groups ?? new List<GroupDto>();
    }
}