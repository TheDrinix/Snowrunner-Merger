using SnowrunnerMerger.Desktop.Models.Groups;

namespace SnowrunnerMerger.Desktop.ViewModels;

public partial class GroupViewModel : ViewModelBase
{
    private readonly Group _group;

    public GroupViewModel(Group group)
    {
        _group = group;
    }
    
    public string Name => _group.Name;
    
    
}