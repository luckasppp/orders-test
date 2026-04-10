using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersApi.Models;

public class Order
{
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }
    public DateTime DataPedido { get; set; }
}