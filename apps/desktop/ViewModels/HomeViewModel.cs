using SnowrunnerMerger.Desktop.Data;

namespace SnowrunnerMerger.Desktop.ViewModels;

public class HomeViewModel : PageViewModel
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    public HomeViewModel()
    {
        Name = PageName.Home;
    }
}