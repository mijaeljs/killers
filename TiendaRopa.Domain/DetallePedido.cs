using System.ComponentModel.DataAnnotations;

namespace TiendaRopa.Domain;

public class DetallePedido
{
    [Key]
    public int IdDetallePedido { get; set; }

    public int IdPedido { get; set; }

    public int IdVariante { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    // Relación con Pedido
    public Pedido Pedido { get; set; } = null!;

    // Relación con ProductoVariante
    public ProductoVariante Variante { get; set; } = null!;
}

