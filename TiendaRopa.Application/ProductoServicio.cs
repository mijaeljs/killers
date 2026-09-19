using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiendaRopa.Domain;
using TiendaRopa.Infraestructura;

namespace TiendaRopa.Application;

public class ProductoServicio
{
    private readonly TiendaRopaContext _context;

    public ProductoServicio(TiendaRopaContext context)
    {
        _context = context;
    }

    public List<Producto> ObtenerProductos()
    {
        return _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Talla)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Color)
            .ToList();
    }

    public Producto? ObtenerPorId(int id)
    {
        return _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Talla)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Color)
            .FirstOrDefault(p => p.IdProducto == id);
    }

    public List<Categoria> ObtenerCategorias()
    {
        return _context.Categorias.ToList();
    }

    public string RegistrarProducto(string nombre, string descripcion, decimal precio, int idCategoria)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return "El nombre del producto es obligatorio.";
        }

        if (precio <= 0)
        {
            return "El precio debe ser mayor a cero.";
        }

        var producto = new Producto
        {
            Nombre = nombre,
            Descripcion = descripcion,
            Precio = precio,
            IdCategoria = idCategoria
        };

        _context.Productos.Add(producto);
        _context.SaveChanges();

        return "Producto registrado correctamente.";
    }
}