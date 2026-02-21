using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowrunnerMerger.Desktop.Interfaces.Stores;

namespace SnowrunnerMerger.Desktop.ViewModels;

public partial class HomePageViewModel(IGroupStore groupStore) : PageViewModel
{
    [ObservableProperty] private ObservableCollection<GroupViewModel> _groups = new();

    [ObservableProperty] private bool _isLoading;

    [RelayCommand]
    private async Task LoadGroupsAsync()
    {
        IsLoading = true;

        await groupStore.LoadGroupsAsync();
        
        Groups = new ObservableCollection<GroupViewModel>(groupStore.Groups.Select(group => new GroupViewModel(group)));
        
        IsLoading = false;
    }
}