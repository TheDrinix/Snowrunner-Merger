using System.Collections.Generic;
using System.Threading.Tasks;
using SnowrunnerMerger.Desktop.Models.Groups;

namespace SnowrunnerMerger.Desktop.Interfaces.Stores;

public interface IGroupStore
{
    IReadOnlyList<Group> Groups { get; }
    Task LoadGroupsAsync(bool forceRefresh = false);
}