using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnowrunnerMerger.Desktop.Interfaces.Services;
using SnowrunnerMerger.Desktop.Interfaces.Stores;
using SnowrunnerMerger.Desktop.Models.Groups;

namespace SnowrunnerMerger.Desktop.Stores;

public class GroupStore(IApiHttpClient httpClient) : IGroupStore
{
    private List<Group> _groups = new();
    private DateTime _lastLoaded = DateTime.MinValue;
    
    public IReadOnlyList<Group> Groups => _groups;
    public async Task LoadGroupsAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && (DateTime.Now - _lastLoaded).TotalMinutes < 5)
            return; // Don't reload if data is fresh and no force refresh requested

        // TODO: Add error handling and logging
        var fetchedGroups = await httpClient.GetGroupsAsync();
        
        _groups = fetchedGroups
            .Select(dto => new Group(dto))
            .ToList();
    }
}