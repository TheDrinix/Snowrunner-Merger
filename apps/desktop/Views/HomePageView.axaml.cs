using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SnowrunnerMerger.Desktop.Views;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();

        Loaded += OnViewLoaded;
    }

    private void OnViewLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.HomePageViewModel vm)
        {
            if (!vm.LoadGroupsCommand.IsRunning)
            {
                vm.LoadGroupsCommand.Execute(null);
            }
        }
    }
}