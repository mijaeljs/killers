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

    public async Task<IActionResult> OnPostActualizarCantidadAsync(int idItem, int cantidad)
    {
        bool esAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                      Request.Headers.Accept.ToString().Contains("application/json");

        if (cantidad < 1)
        {
            if (esAjax)
                return new JsonResult(new { exito = false, mensaje = "La cantidad mínima permitida es 1." });

            TempData["Error"] = "La cantidad mínima permitida es 1.";
            return RedirectToPage();
        }

        var resultado = await _carritoServicio.ActualizarCantidadAsync(idItem, cantidad);

        if (esAjax)
        {
            if (!resultado.Exito)
            {
                return new JsonResult(new { exito = false, mensaje = resultado.Mensaje });
            }

            if (Request.Cookies.TryGetValue("IdClienteActivo", out string? idClienteStr) && int.TryParse(idClienteStr, out int idCliente))
            {
                var carrito = await _carritoServicio.ObtenerCarritoPorClienteAsync(idCliente);
                var itemModificado = carrito?.Items.FirstOrDefault(i => i.IdItemCarrito == idItem);
                decimal itemSubtotal = (itemModificado?.Variante?.Producto?.Precio ?? 0) * (itemModificado?.Cantidad ?? 0);
                decimal nuevoTotal = carrito?.Items.Sum(i => (i.Variante?.Producto?.Precio ?? 0) * i.Cantidad) ?? 0;

                return new JsonResult(new
                {
                    exito = true,
                    mensaje = resultado.Mensaje,
                    cantidad = itemModificado?.Cantidad ?? cantidad,
                    subtotal = itemSubtotal.ToString("F2"),
                    total = nuevoTotal.ToString("F2")
                });
            }

            return new JsonResult(new { exito = true, mensaje = resultado.Mensaje });
        }

        if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Mensaje;
        }
        else
        {
            TempData["Mensaje"] = resultado.Mensaje;
        }

        return RedirectToPage();
    }
}