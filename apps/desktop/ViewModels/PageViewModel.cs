using CommunityToolkit.Mvvm.ComponentModel;
using SnowrunnerMerger.Desktop.Data;

namespace SnowrunnerMerger.Desktop.ViewModels;

public abstract partial class PageViewModel : ViewModelBase
{
    [ObservableProperty] private PageName _name;
}