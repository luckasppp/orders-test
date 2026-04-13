using MongoDB.Driver;
using OrdersApi.Models;

namespace OrdersApi.Repositories;

public class MongoOrderCacheRepository : IOrderCacheRepository
{
    private readonly IMongoCollection<CachedOrder> _collection;

    public MongoOrderCacheRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("OrdersCache");
        _collection = database.GetCollection<CachedOrder>("orders");
    }

    public async Task<CachedOrder?> GetByIdAsync(int id)
    {
        var filter = Builders<CachedOrder>.Filter.Eq(o => o.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task SetAsync(CachedOrder order)
    {
        var filter = Builders<CachedOrder>.Filter.Eq(o => o.Id, order.Id);
        var options = new ReplaceOptions { IsUpsert = true };
        await _collection.ReplaceOneAsync(filter, order, options);
    }
}