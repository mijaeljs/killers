using Microsoft.EntityFrameworkCore;
using TiendaRopa.Domain;
using TiendaRopa.Infraestructura;

namespace TiendaRopa.Application;

public class CarritoServicio
{
    private readonly TiendaRopaContext _context;

    public CarritoServicio(TiendaRopaContext context)
    {
        _context = context;
    }

    // Registrar un nuevo cliente y asignarle un carrito vacío
    public async Task<Cliente> RegistrarClienteAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        var carrito = new Carrito
        {
            IdCliente = cliente.IdCliente,
            FechaCreacion = DateTime.Now
        };

        _context.Carritos.Add(carrito);
        await _context.SaveChangesAsync();

        return cliente;
    }

    public async Task<Cliente?> ObtenerClienteAsync(int idCliente)
    {
        return await _context.Clientes.FindAsync(idCliente);
    }

    // Obtener el carrito activo con sus ítems, producto, talla y color
    public async Task<Carrito?> ObtenerCarritoPorClienteAsync(int idCliente)
    {
        return await _context.Carritos
            .Include(c => c.Items)
                .ThenInclude(i => i.Variante)
                    .ThenInclude(v => v!.Producto)
            .Include(c => c.Items)
                .ThenInclude(i => i.Variante)
                    .ThenInclude(v => v!.Talla)
            .Include(c => c.Items)
                .ThenInclude(i => i.Variante)
                    .ThenInclude(v => v!.Color)
            .FirstOrDefaultAsync(c => c.IdCliente == idCliente);
    }

    // Agregar producto al carrito validando el stock
    public async Task<(bool Exito, string Mensaje)> AgregarAlCarritoAsync(int idCliente, int idVariante, int cantidad)
    {
        var variante = await _context.ProductoVariantes.FindAsync(idVariante);
        if (variante == null)
            return (false, "La variante seleccionada no existe.");

        if (cantidad > variante.Stock)
            return (false, $"Stock insuficiente. Solo quedan {variante.Stock} unidades disponibles.");

        var carrito = await _context.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

        if (carrito == null)
            return (false, "El cliente no tiene un carrito asignado.");

        var itemExistente = carrito.Items.FirstOrDefault(i => i.IdVariante == idVariante);

        if (itemExistente != null)
        {
            if (itemExistente.Cantidad + cantidad > variante.Stock)
                return (false, $"No puedes agregar más. El stock máximo es de {variante.Stock} unidades.");

            itemExistente.Cantidad += cantidad;
        }
        else
        {
            carrito.Items.Add(new ItemCarrito
            {
                IdCarrito = carrito.IdCarrito,
                IdVariante = idVariante,
                Cantidad = cantidad
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Producto agregado al carrito con éxito.");
    }

    // Quitar un ítem del carrito
    public async Task QuitarDelCarritoAsync(int idItemCarrito)
    {
        var item = await _context.ItemsCarrito.FindAsync(idItemCarrito);
        if (item != null)
        {
            _context.ItemsCarrito.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    // Actualizar cantidad de un ítem validando stock
    public async Task<(bool Exito, string Mensaje)> ActualizarCantidadAsync(int idItemCarrito, int nuevaCantidad)
    {
        if (nuevaCantidad < 1)
            return (false, "La cantidad mínima es 1.");

        var item = await _context.ItemsCarrito
            .Include(i => i.Variante)
            .FirstOrDefaultAsync(i => i.IdItemCarrito == idItemCarrito);

        if (item == null)
            return (false, "El ítem no existe en el carrito.");

        if (item.Variante == null)
            return (false, "La variante del producto no fue encontrada.");

        if (nuevaCantidad > item.Variante.Stock)
            return (false, $"Stock insuficiente. Solo quedan {item.Variante.Stock} unidades disponibles.");

        item.Cantidad = nuevaCantidad;
        await _context.SaveChangesAsync();

        return (true, "Cantidad actualizada correctamente.");
    }
}