using Microsoft.EntityFrameworkCore;
using TiendaRopa.Domain;
using TiendaRopa.Infraestructura;

namespace TiendaRopa.Application;

public class PedidoServicio
{
    private readonly TiendaRopaContext _context;

    public PedidoServicio(TiendaRopaContext context)
    {
        _context = context;
    }

    // Confirmar y crear el pedido a partir del carrito del cliente
    public async Task<(bool Exito, string Mensaje, Pedido? Pedido)> CrearPedidoAsync(int idCliente)
    {
        var cliente = await _context.Clientes.FindAsync(idCliente);
        if (cliente == null)
            return (false, "El cliente no existe.", null);

        var carrito = await _context.Carritos
            .Include(c => c.Items)
                .ThenInclude(i => i.Variante)
                    .ThenInclude(v => v!.Producto)
            .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

        if (carrito == null || !carrito.Items.Any())
            return (false, "El carrito de compras está vacío.", null);

        // 1. Validar stock actual en base de datos para todos los ítems antes de proceder
        foreach (var item in carrito.Items)
        {
            if (item.Variante == null)
                return (false, "Una o más prendas seleccionadas ya no están disponibles.", null);

            if (item.Cantidad > item.Variante.Stock)
            {
                var nombreProducto = item.Variante.Producto?.Nombre ?? "Producto";
                return (false, $"Stock insuficiente para {nombreProducto}. Stock disponible: {item.Variante.Stock} unidad(es).", null);
            }
        }

        // 2. Calcular total a partir de precios actualizados en la base de datos
        decimal total = carrito.Items.Sum(i => (i.Variante?.Producto?.Precio ?? 0) * i.Cantidad);

        // 3. Crear el pedido
        var pedido = new Pedido
        {
            IdCliente = idCliente,
            FechaPedido = DateTime.Now,
            Total = total,
            Estado = "Pendiente"
        };

        // 4. Crear los detalles y descontar el stock
        foreach (var item in carrito.Items)
        {
            var precioUnitario = item.Variante?.Producto?.Precio ?? 0;

            pedido.Detalles.Add(new DetallePedido
            {
                IdVariante = item.IdVariante,
                Cantidad = item.Cantidad,
                PrecioUnitario = precioUnitario
            });

            // Descontar stock
            item.Variante!.Stock -= item.Cantidad;
        }

        _context.Pedidos.Add(pedido);

        // 5. Vaciar carrito
        _context.ItemsCarrito.RemoveRange(carrito.Items);

        // 6. Guardar cambios transaccionales
        await _context.SaveChangesAsync();

        return (true, "Pedido generado con éxito.", pedido);
    }

    // Obtener historial de pedidos de un cliente
    public async Task<List<Pedido>> ObtenerPedidosPorClienteAsync(int idCliente)
    {
        return await _context.Pedidos
            .Where(p => p.IdCliente == idCliente)
            .Include(p => p.Detalles)
            .OrderByDescending(p => p.FechaPedido)
            .ToListAsync();
    }

    // Obtener detalle completo de un pedido específico
    public async Task<Pedido?> ObtenerDetallePedidoAsync(int idPedido, int? idCliente = null)
    {
        var query = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Variante)
                    .ThenInclude(v => v.Producto)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Variante)
                    .ThenInclude(v => v.Talla)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Variante)
                    .ThenInclude(v => v.Color)
            .AsQueryable();

        if (idCliente.HasValue)
        {
            query = query.Where(p => p.IdCliente == idCliente.Value);
        }

        return await query.FirstOrDefaultAsync(p => p.IdPedido == idPedido);
    }

    // Listar todos los pedidos (para módulo de administración)
    public async Task<List<Pedido>> ObtenerTodosLosPedidosAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Detalles)
            .OrderByDescending(p => p.FechaPedido)
            .ToListAsync();
    }

    // Cambiar estado del pedido
    public async Task<bool> CambiarEstadoAsync(int idPedido, string nuevoEstado)
    {
        var pedido = await _context.Pedidos.FindAsync(idPedido);
        if (pedido == null)
            return false;

        pedido.Estado = nuevoEstado;
        await _context.SaveChangesAsync();
        return true;
    }
}
