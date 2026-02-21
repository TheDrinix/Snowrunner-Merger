using System.ComponentModel;
using SnowrunnerMerger.Desktop.Interfaces.Services;

namespace SnowrunnerMerger.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IRouterService _routerService;
    
    public MainWindowViewModel(IRouterService routerService)
    {
        _routerService = routerService;
        _routerService.PropertyChanged += OnRouterPropertyChanged;
    }
    
    public PageViewModel CurrentView => _routerService.CurrentView;

    private void OnRouterPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_routerService.CurrentView))
        {
            OnPropertyChanged(nameof(CurrentView));
        }
    }
}