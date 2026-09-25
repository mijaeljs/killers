using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Carrito;

public class IndexModel : PageModel
{
    private readonly CarritoServicio _carritoServicio;

    public IndexModel(CarritoServicio carritoServicio)
    {
        _carritoServicio = carritoServicio;
    }

    public Domain.Carrito? Carrito { get; set; }
    public decimal Total { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!Request.Cookies.TryGetValue("IdClienteActivo", out string? idClienteStr) || !int.TryParse(idClienteStr, out int idCliente))
        {
            return RedirectToPage("/Clientes/Registrar");
        }

        Carrito = await _carritoServicio.ObtenerCarritoPorClienteAsync(idCliente);

        if (Carrito != null && Carrito.Items.Any())
        {
            Total = Carrito.Items.Sum(i => (i.Variante?.Producto?.Precio ?? 0) * i.Cantidad);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int idItem)
    {
        await _carritoServicio.QuitarDelCarritoAsync(idItem);
        TempData["Mensaje"] = "Producto quitado del carrito.";
        return RedirectToPage();
    }
}