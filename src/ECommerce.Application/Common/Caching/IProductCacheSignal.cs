using System.Threading;

namespace ECommerce.Application.Common.Caching;

public interface IProductCacheSignal
{
    CancellationToken Token { get; }
    void Reset();
}
