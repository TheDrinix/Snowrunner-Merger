using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SnowrunnerMerger.Desktop.Interfaces;

namespace SnowrunnerMerger.Desktop.Utils.Browser;

public class SystemBrowser(int port = 7890) : IBrowser
{
    public async Task<string> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add(options.EndUrl);
        listener.Start();

        Process.Start(new ProcessStartInfo()
        {
            FileName = options.StartUrl,
            UseShellExecute = true,
        });
        
        var context = await listener.GetContextAsync();

        var res = context.Response;
        var resString = "<html><body><h1>Login successful! You can close this window.</h1></body></html>";
        var buffer = System.Text.Encoding.UTF8.GetBytes(resString);
        res.ContentLength64 = buffer.Length;
        
        var resOutput = res.OutputStream;
        await resOutput.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        resOutput.Close();

        return context.Request.Url?.ToString() ?? "";
    }
}