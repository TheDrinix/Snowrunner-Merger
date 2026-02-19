using System.Threading;
using System.Threading.Tasks;

namespace SnowrunnerMerger.Desktop.Utils.Browser;

public interface IBrowser
{
    Task<string> InvokeAsync(BrowserOptions options,
        CancellationToken cancellationToken = new CancellationToken());
}