using Microsoft.EntityFrameworkCore;
using OrdersApi.Domain.Entities;
using OrdersApi.Domain.Repositories;

namespace OrdersApi.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders
            .OrderByDescending(o => o.DataPedido)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders.FindAsync(id);
    }

    public async Task AddAsync(Order order)
    {
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
    }
}