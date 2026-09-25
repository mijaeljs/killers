using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Productos;

public class IndexModel : PageModel
{
    private readonly ProductoServicio _productoServicio;
    private readonly CarritoServicio _carritoServicio;

    public IndexModel(ProductoServicio productoServicio, CarritoServicio carritoServicio)
    {
        _productoServicio = productoServicio;
        _carritoServicio = carritoServicio;
    }

    public List<Producto> Productos { get; set; } = new();

    public void OnGet()
    {
        Productos = _productoServicio.ObtenerProductos();
    }

    public async Task<IActionResult> OnPostAgregarAsync(int idVariante, int cantidad)
    {
        if (!Request.Cookies.TryGetValue("IdClienteActivo", out string? idClienteStr) || !int.TryParse(idClienteStr, out int idCliente))
        {
            TempData["Mensaje"] = "Por favor identifícate o regístrate para comenzar a comprar.";
            return RedirectToPage("/Clientes/Registrar");
        }

        if (cantidad <= 0)
        {
            TempData["Error"] = "La cantidad debe ser mayor a 0.";
            return RedirectToPage();
        }

        var resultado = await _carritoServicio.AgregarAlCarritoAsync(idCliente, idVariante, cantidad);

        if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Mensaje;
            return RedirectToPage();
        }

        TempData["Mensaje"] = resultado.Mensaje;
        return RedirectToPage("/Carrito/Index");
    }
}