using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Productos;

public class IndexModel : PageModel
{
    private readonly ProductoServicio _productoServicio;

    public IndexModel(ProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    public List<Producto> Productos { get; set; } = new();

    public void OnGet()
    {
        Productos = _productoServicio.ObtenerProductos();
    }
}