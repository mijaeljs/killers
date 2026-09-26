using Microsoft.EntityFrameworkCore;
using TiendaRopa.Domain;
using TiendaRopa.Infraestructura;

namespace TiendaRopa.Application;

public class DashboardResumenDto
{
    public decimal TotalVentas { get; set; }
    public int TotalPedidos { get; set; }
    public int TotalClientes { get; set; }
    public int TotalProductos { get; set; }
    public int TotalVariantes { get; set; }
    public int PedidosPendientes { get; set; }
    public int PedidosPreparando { get; set; }
    public int PedidosListos { get; set; }
    public int PedidosEntregados { get; set; }
    public int PedidosCancelados { get; set; }
    public int ProductosStockBajo { get; set; }
    public int ProductosSinStock { get; set; }
    public List<Pedido> UltimosPedidos { get; set; } = new();
    public List<ProductoVariante> AlertasStock { get; set; } = new();
}

public class DashboardServicio
{
    private readonly TiendaRopaContext _context;

    public DashboardServicio(TiendaRopaContext context)
    {
        _context = context;
    }

    public async Task<DashboardResumenDto> ObtenerResumenAsync()
    {
        var resumen = new DashboardResumenDto();

        resumen.TotalProductos = await _context.Productos.CountAsync();
        resumen.TotalClientes = await _context.Clientes.CountAsync();
        resumen.TotalPedidos = await _context.Pedidos.CountAsync();
        resumen.TotalVariantes = await _context.ProductoVariantes.CountAsync();

        // Ventas acumuladas de pedidos no cancelados
        resumen.TotalVentas = await _context.Pedidos
            .Where(p => p.Estado != "Cancelado")
            .SumAsync(p => p.Total);

        // Desglose por estados de pedido
        resumen.PedidosPendientes = await _context.Pedidos.CountAsync(p => p.Estado == "Pendiente");
        resumen.PedidosPreparando = await _context.Pedidos.CountAsync(p => p.Estado == "Preparando");
        resumen.PedidosListos = await _context.Pedidos.CountAsync(p => p.Estado == "Listo");
        resumen.PedidosEntregados = await _context.Pedidos.CountAsync(p => p.Estado == "Entregado");
        resumen.PedidosCancelados = await _context.Pedidos.CountAsync(p => p.Estado == "Cancelado");

        // Alertas de Stock
        resumen.ProductosStockBajo = await _context.ProductoVariantes.CountAsync(v => v.Stock > 0 && v.Stock <= 5);
        resumen.ProductosSinStock = await _context.ProductoVariantes.CountAsync(v => v.Stock == 0);

        // Últimos 5 pedidos registrados
        resumen.UltimosPedidos = await _context.Pedidos
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.FechaPedido)
            .Take(5)
            .ToListAsync();

        // Prendas críticas de stock para panel de alertas
        resumen.AlertasStock = await _context.ProductoVariantes
            .Include(v => v.Producto)
            .Include(v => v.Talla)
            .Include(v => v.Color)
            .Where(v => v.Stock <= 5)
            .OrderBy(v => v.Stock)
            .Take(6)
            .ToListAsync();

        return resumen;
    }
}
