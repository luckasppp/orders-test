using MongoDB.Bson.Serialization.Attributes;

namespace OrdersApi.Models;

public class CachedOrder
{
    [BsonId]
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataPedido { get; set; }
}