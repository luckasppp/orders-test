using OrdersApi.Infrastructure.Cache;

namespace OrdersApi.Domain.Repositories;

public interface IOrderCacheRepository
{
    Task<CachedOrder?> GetByIdAsync(int id);
    Task SetAsync(CachedOrder order);
}