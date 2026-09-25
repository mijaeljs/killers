using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class Pedido
{
    [Key]
    public int IdPedido { get; set; }

    public int IdCliente { get; set; }

    public DateTime FechaPedido { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = "Pendiente";

    // Relación con Cliente
    public Cliente Cliente { get; set; } = null!;

    // Relación 1 - N con DetallePedido
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}