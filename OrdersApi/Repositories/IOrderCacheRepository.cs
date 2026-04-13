using OrdersApi.Models;

namespace OrdersApi.Repositories;

public interface IOrderCacheRepository
{
    Task<CachedOrder?> GetByIdAsync(int id);
    Task SetAsync(CachedOrder order);
}