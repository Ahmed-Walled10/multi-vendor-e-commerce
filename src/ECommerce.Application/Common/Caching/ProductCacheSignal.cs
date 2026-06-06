using System.Threading;

namespace ECommerce.Application.Common.Caching;

public class ProductCacheSignal : IProductCacheSignal
{
    private CancellationTokenSource _cts = new();

    public CancellationToken Token => _cts.Token;

    public void Reset()
    {
        var oldCts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
        try
        {
            oldCts.Cancel();
        }
        finally
        {
            oldCts.Dispose();
        }
    }
}
