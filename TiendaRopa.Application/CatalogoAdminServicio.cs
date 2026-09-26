using Microsoft.EntityFrameworkCore;
using TiendaRopa.Domain;
using TiendaRopa.Infraestructura;

namespace TiendaRopa.Application;

public class ClienteAdminDto
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public int TotalPedidos { get; set; }
    public decimal TotalGastado { get; set; }
    public DateTime? UltimaCompra { get; set; }
}

public class CatalogoAdminServicio
{
    private readonly TiendaRopaContext _context;

    public CatalogoAdminServicio(TiendaRopaContext context)
    {
        _context = context;
    }

    // ==========================================
    // 1. CATEGORÍAS
    // ==========================================
    public async Task<List<Categoria>> ListarCategoriasAsync()
    {
        return await _context.Categorias
            .Include(c => c.Productos)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int id)
    {
        return await _context.Categorias
            .Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.IdCategoria == id);
    }

    public async Task<(bool Exito, string Mensaje)> CrearCategoriaAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return (false, "El nombre de la categoría es obligatorio.");

        nombre = nombre.Trim();
        var existe = await _context.Categorias.AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        if (existe)
            return (false, "Ya existe una categoría con ese nombre.");

        var categoria = new Categoria { Nombre = nombre };
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return (true, "Categoría creada con éxito.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarCategoriaAsync(int id, string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            return (false, "El nombre de la categoría es obligatorio.");

        nuevoNombre = nuevoNombre.Trim();
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return (false, "La categoría no existe.");

        var existeOtro = await _context.Categorias.AnyAsync(c => c.IdCategoria != id && c.Nombre.ToLower() == nuevoNombre.ToLower());
        if (existeOtro)
            return (false, "Ya existe otra categoría con ese nombre.");

        categoria.Nombre = nuevoNombre;
        await _context.SaveChangesAsync();

        return (true, "Categoría actualizada correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarCategoriaAsync(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.IdCategoria == id);

        if (categoria == null)
            return (false, "La categoría no existe.");

        if (categoria.Productos.Any())
            return (false, $"No se puede eliminar la categoría porque tiene {categoria.Productos.Count} producto(s) asignado(s). Reasigne o elimine los productos primero.");

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return (true, "Categoría eliminada con éxito.");
    }

    // ==========================================
    // 2. TALLAS
    // ==========================================
    public async Task<List<Talla>> ListarTallasAsync()
    {
        return await _context.Tallas
            .Include(t => t.Variantes)
            .OrderBy(t => t.Nombre)
            .ToListAsync();
    }

    public async Task<Talla?> ObtenerTallaPorIdAsync(int id)
    {
        return await _context.Tallas.FindAsync(id);
    }

    public async Task<(bool Exito, string Mensaje)> CrearTallaAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return (false, "El nombre de la talla es obligatorio.");

        nombre = nombre.Trim().ToUpper();
        var existe = await _context.Tallas.AnyAsync(t => t.Nombre.ToUpper() == nombre);
        if (existe)
            return (false, "Ya existe una talla con ese nombre.");

        var talla = new Talla { Nombre = nombre };
        _context.Tallas.Add(talla);
        await _context.SaveChangesAsync();

        return (true, "Talla creada correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarTallaAsync(int id, string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            return (false, "El nombre de la talla es obligatorio.");

        nuevoNombre = nuevoNombre.Trim().ToUpper();
        var talla = await _context.Tallas.FindAsync(id);
        if (talla == null)
            return (false, "La talla no existe.");

        var existeOtro = await _context.Tallas.AnyAsync(t => t.IdTalla != id && t.Nombre.ToUpper() == nuevoNombre);
        if (existeOtro)
            return (false, "Ya existe otra talla con ese nombre.");

        talla.Nombre = nuevoNombre;
        await _context.SaveChangesAsync();

        return (true, "Talla actualizada con éxito.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarTallaAsync(int id)
    {
        var talla = await _context.Tallas
            .Include(t => t.Variantes)
            .FirstOrDefaultAsync(t => t.IdTalla == id);

        if (talla == null)
            return (false, "La talla no existe.");

        if (talla.Variantes.Any())
            return (false, $"No se puede eliminar la talla porque está vinculada a {talla.Variantes.Count} variante(s) de productos.");

        _context.Tallas.Remove(talla);
        await _context.SaveChangesAsync();

        return (true, "Talla eliminada con éxito.");
    }

    // ==========================================
    // 3. COLORES
    // ==========================================
    public async Task<List<Color>> ListarColoresAsync()
    {
        return await _context.Colores
            .Include(c => c.Variantes)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Color?> ObtenerColorPorIdAsync(int id)
    {
        return await _context.Colores.FindAsync(id);
    }

    public async Task<(bool Exito, string Mensaje)> CrearColorAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return (false, "El nombre del color es obligatorio.");

        nombre = nombre.Trim();
        var existe = await _context.Colores.AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        if (existe)
            return (false, "Ya existe un color con ese nombre.");

        var color = new Color { Nombre = nombre };
        _context.Colores.Add(color);
        await _context.SaveChangesAsync();

        return (true, "Color creado correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarColorAsync(int id, string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            return (false, "El nombre del color es obligatorio.");

        nuevoNombre = nuevoNombre.Trim();
        var color = await _context.Colores.FindAsync(id);
        if (color == null)
            return (false, "El color no existe.");

        var existeOtro = await _context.Colores.AnyAsync(c => c.IdColor != id && c.Nombre.ToLower() == nuevoNombre.ToLower());
        if (existeOtro)
            return (false, "Ya existe otro color con ese nombre.");

        color.Nombre = nuevoNombre;
        await _context.SaveChangesAsync();

        return (true, "Color actualizado con éxito.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarColorAsync(int id)
    {
        var color = await _context.Colores
            .Include(c => c.Variantes)
            .FirstOrDefaultAsync(c => c.IdColor == id);

        if (color == null)
            return (false, "El color no existe.");

        if (color.Variantes.Any())
            return (false, $"No se puede eliminar el color porque está vinculado a {color.Variantes.Count} variante(s) de productos.");

        _context.Colores.Remove(color);
        await _context.SaveChangesAsync();

        return (true, "Color eliminado con éxito.");
    }

    // ==========================================
    // 4. PRODUCTOS
    // ==========================================
    public async Task<List<Producto>> ListarProductosAsync(string? buscar = null, int? idCategoria = null)
    {
        var query = _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Talla)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Color)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.Trim().ToLower();
            query = query.Where(p => p.Nombre.ToLower().Contains(term) || p.Descripcion.ToLower().Contains(term));
        }

        if (idCategoria.HasValue && idCategoria.Value > 0)
        {
            query = query.Where(p => p.IdCategoria == idCategoria.Value);
        }

        return await query.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Producto?> ObtenerProductoPorIdAsync(int id)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Talla)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Color)
            .FirstOrDefaultAsync(p => p.IdProducto == id);
    }

    public async Task<(bool Exito, string Mensaje, int IdProducto)> CrearProductoAsync(string nombre, string descripcion, decimal precio, int idCategoria)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return (false, "El nombre del producto es obligatorio.", 0);

        if (precio <= 0)
            return (false, "El precio debe ser mayor a 0.", 0);

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == idCategoria);
        if (!categoriaExiste)
            return (false, "La categoría seleccionada no existe.", 0);

        var producto = new Producto
        {
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim() ?? string.Empty,
            Precio = precio,
            IdCategoria = idCategoria
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return (true, "Producto creado exitosamente.", producto.IdProducto);
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarProductoAsync(int id, string nombre, string descripcion, decimal precio, int idCategoria)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return (false, "El nombre del producto es obligatorio.");

        if (precio <= 0)
            return (false, "El precio debe ser mayor a 0.");

        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
            return (false, "El producto no existe.");

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == idCategoria);
        if (!categoriaExiste)
            return (false, "La categoría seleccionada no existe.");

        producto.Nombre = nombre.Trim();
        producto.Descripcion = descripcion?.Trim() ?? string.Empty;
        producto.Precio = precio;
        producto.IdCategoria = idCategoria;

        await _context.SaveChangesAsync();

        return (true, "Producto actualizado correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarProductoAsync(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.Variantes)
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        if (producto == null)
            return (false, "El producto no existe.");

        // Verificar si alguna variante tiene detalles de pedidos históricos
        var idsVariantes = producto.Variantes.Select(v => v.IdVariante).ToList();
        var tienePedidos = await _context.DetallesPedido.AnyAsync(d => idsVariantes.Contains(d.IdVariante));
        if (tienePedidos)
        {
            return (false, "No se puede eliminar el producto porque tiene pedidos asociados en el historial de ventas.");
        }

        // Eliminar items de carrito pendientes que contengan estas variantes
        var itemsCarrito = await _context.ItemsCarrito.Where(i => idsVariantes.Contains(i.IdVariante)).ToListAsync();
        if (itemsCarrito.Any())
        {
            _context.ItemsCarrito.RemoveRange(itemsCarrito);
        }

        _context.ProductoVariantes.RemoveRange(producto.Variantes);
        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return (true, "Producto y sus variantes eliminados exitosamente.");
    }

    // ==========================================
    // 5. VARIANTES Y STOCK
    // ==========================================
    public async Task<List<ProductoVariante>> ListarVariantesAsync(int? idProducto = null, string? buscar = null)
    {
        var query = _context.ProductoVariantes
            .Include(v => v.Producto)
                .ThenInclude(p => p!.Categoria)
            .Include(v => v.Talla)
            .Include(v => v.Color)
            .AsQueryable();

        if (idProducto.HasValue && idProducto.Value > 0)
        {
            query = query.Where(v => v.IdProducto == idProducto.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.Trim().ToLower();
            query = query.Where(v =>
                v.Producto!.Nombre.ToLower().Contains(term) ||
                v.Talla!.Nombre.ToLower().Contains(term) ||
                v.Color!.Nombre.ToLower().Contains(term));
        }

        return await query
            .OrderBy(v => v.Producto!.Nombre)
            .ThenBy(v => v.Talla!.Nombre)
            .ToListAsync();
    }

    public async Task<ProductoVariante?> ObtenerVariantePorIdAsync(int idVariante)
    {
        return await _context.ProductoVariantes
            .Include(v => v.Producto)
            .Include(v => v.Talla)
            .Include(v => v.Color)
            .FirstOrDefaultAsync(v => v.IdVariante == idVariante);
    }

    public async Task<(bool Exito, string Mensaje)> CrearVarianteAsync(int idProducto, int idTalla, int idColor, int stock)
    {
        if (stock < 0)
            return (false, "El stock no puede ser negativo.");

        var producto = await _context.Productos.FindAsync(idProducto);
        if (producto == null)
            return (false, "El producto no existe.");

        var talla = await _context.Tallas.FindAsync(idTalla);
        if (talla == null)
            return (false, "La talla no existe.");

        var color = await _context.Colores.FindAsync(idColor);
        if (color == null)
            return (false, "El color no existe.");

        // Evitar duplicar la misma combinación de producto + talla + color
        var yaExiste = await _context.ProductoVariantes.AnyAsync(v =>
            v.IdProducto == idProducto && v.IdTalla == idTalla && v.IdColor == idColor);

        if (yaExiste)
            return (false, "Ya existe una variante con esa misma combinación de Talla y Color para este producto.");

        var variante = new ProductoVariante
        {
            IdProducto = idProducto,
            IdTalla = idTalla,
            IdColor = idColor,
            Stock = stock
        };

        _context.ProductoVariantes.Add(variante);
        await _context.SaveChangesAsync();

        return (true, "Variante creada exitosamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarVarianteAsync(int idVariante, int idTalla, int idColor, int stock)
    {
        if (stock < 0)
            return (false, "El stock no puede ser negativo.");

        var variante = await _context.ProductoVariantes.FindAsync(idVariante);
        if (variante == null)
            return (false, "La variante no existe.");

        var yaExiste = await _context.ProductoVariantes.AnyAsync(v =>
            v.IdVariante != idVariante &&
            v.IdProducto == variante.IdProducto &&
            v.IdTalla == idTalla &&
            v.IdColor == idColor);

        if (yaExiste)
            return (false, "Ya existe otra variante con esa misma combinación de Talla y Color.");

        variante.IdTalla = idTalla;
        variante.IdColor = idColor;
        variante.Stock = stock;

        await _context.SaveChangesAsync();

        return (true, "Variante actualizada exitosamente.");
    }

    public async Task<(bool Exito, string Mensaje)> ActualizarStockAsync(int idVariante, int nuevoStock)
    {
        if (nuevoStock < 0)
            return (false, "El stock no puede ser negativo.");

        var variante = await _context.ProductoVariantes
            .Include(v => v.Producto)
            .Include(v => v.Talla)
            .Include(v => v.Color)
            .FirstOrDefaultAsync(v => v.IdVariante == idVariante);

        if (variante == null)
            return (false, "La variante no existe.");

        variante.Stock = nuevoStock;
        await _context.SaveChangesAsync();

        return (true, $"Stock de '{variante.Producto?.Nombre} ({variante.Talla?.Nombre} / {variante.Color?.Nombre})' actualizado a {nuevoStock}.");
    }

    public async Task<(bool Exito, string Mensaje)> EliminarVarianteAsync(int idVariante)
    {
        var variante = await _context.ProductoVariantes.FindAsync(idVariante);
        if (variante == null)
            return (false, "La variante no existe.");

        var tienePedidos = await _context.DetallesPedido.AnyAsync(d => d.IdVariante == idVariante);
        if (tienePedidos)
            return (false, "No se puede eliminar la variante porque está asociada a pedidos históricos. Puede ajustar su stock a 0 en su lugar.");

        var itemsCarrito = await _context.ItemsCarrito.Where(i => i.IdVariante == idVariante).ToListAsync();
        if (itemsCarrito.Any())
        {
            _context.ItemsCarrito.RemoveRange(itemsCarrito);
        }

        _context.ProductoVariantes.Remove(variante);
        await _context.SaveChangesAsync();

        return (true, "Variante eliminada exitosamente.");
    }

    // ==========================================
    // 6. CLIENTES (CONSULTA ADMINISTRATIVA)
    // ==========================================
    public async Task<List<ClienteAdminDto>> ListarClientesConEstadisticasAsync(string? buscar = null)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.Trim().ToLower();
            query = query.Where(c => c.Nombre.ToLower().Contains(term) ||
                                     c.Correo.ToLower().Contains(term) ||
                                     c.Telefono.ToLower().Contains(term));
        }

        var clientes = await query.ToListAsync();

        var pedidosPorCliente = await _context.Pedidos
            .GroupBy(p => p.IdCliente)
            .Select(g => new
            {
                IdCliente = g.Key,
                TotalPedidos = g.Count(),
                TotalGastado = g.Where(p => p.Estado != "Cancelado").Sum(p => p.Total),
                UltimaFecha = g.Max(p => p.FechaPedido)
            })
            .ToDictionaryAsync(x => x.IdCliente);

        var resultado = clientes.Select(c =>
        {
            pedidosPorCliente.TryGetValue(c.IdCliente, out var resumen);
            return new ClienteAdminDto
            {
                IdCliente = c.IdCliente,
                Nombre = c.Nombre,
                Correo = c.Correo,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                TotalPedidos = resumen?.TotalPedidos ?? 0,
                TotalGastado = resumen?.TotalGastado ?? 0,
                UltimaCompra = resumen?.UltimaFecha
            };
        }).OrderByDescending(c => c.TotalGastado).ToList();

        return resultado;
    }

    public async Task<Cliente?> ObtenerDetalleClienteAsync(int idCliente)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.IdCliente == idCliente);
    }
}
