using System;

namespace SnowrunnerMerger.Desktop.Utils.Browser;

public class BrowserOptions(string startUrl, string endUrl)
{
    public string StartUrl { get; } = startUrl;
    public string EndUrl { get; } = endUrl;
    
    public TimeSpan TimeOut { get; set; } = TimeSpan.FromMinutes(5);
}