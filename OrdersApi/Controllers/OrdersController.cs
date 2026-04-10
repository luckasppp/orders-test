using Microsoft.AspNetCore.Mvc;
using OrdersApi.Dtos;
using OrdersApi.Models;
using OrdersApi.Repositories;

namespace OrdersApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _repository;

    public OrdersController(IOrderRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _repository.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _repository.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var order = new Order
        {
            Cliente = dto.Cliente,
            Valor = dto.Valor,
            DataPedido = DateTime.UtcNow
        };

        await _repository.AddAsync(order);

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
}