using System.ComponentModel;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.ViewModels;

namespace SnowrunnerMerger.Desktop.Services.Interfaces;

public interface IRouterService : INotifyPropertyChanged
{
    public PageViewModel CurrentView { get; set; }
    public void NavigateTo(PageName pageName);
}