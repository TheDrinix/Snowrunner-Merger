using System.ComponentModel;
using SnowrunnerMerger.Desktop.ViewModels;

namespace SnowrunnerMerger.Desktop.Interfaces.Services;

public interface IRouterService : INotifyPropertyChanged
{
    public PageViewModel CurrentView { get; }
    public void NavigateTo<TViewModel>() where TViewModel : PageViewModel;
}