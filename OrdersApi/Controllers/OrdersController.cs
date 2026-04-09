using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;

namespace OrdersApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private static readonly List<Order> _ordersFake = new()
    {
        new Order { Id = 1, Cliente = "João Silva", Valor = 150.00m, DataPedido = DateTime.UtcNow.AddDays(-2) },
        new Order { Id = 2, Cliente = "Maria Souza", Valor = 89.00m, DataPedido = DateTime.UtcNow.AddDays(-1) },
        new Order { Id = 3, Cliente = "Pedro Souza", Valor = 1200.00m, DataPedido = DateTime.UtcNow }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_ordersFake);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var order = _ordersFake.FirstOrDefault( o => o.Id == id );

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}
