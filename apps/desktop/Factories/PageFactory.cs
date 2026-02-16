using System;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.ViewModels;

namespace SnowrunnerMerger.Desktop.Factories;

public class PageFactory(Func<PageName, PageViewModel> factory)
{
    public PageViewModel GetPageModel(PageName name) => factory(name);
}