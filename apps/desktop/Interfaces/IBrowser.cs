using System.Threading;
using System.Threading.Tasks;
using SnowrunnerMerger.Desktop.Utils.Browser;

namespace SnowrunnerMerger.Desktop.Interfaces;

public interface IBrowser
{
    Task<string> InvokeAsync(BrowserOptions options,
        CancellationToken cancellationToken = new CancellationToken());
}