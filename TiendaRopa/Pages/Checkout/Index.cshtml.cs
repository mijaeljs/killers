using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Checkout;

public class IndexModel : PageModel
{
    private readonly CarritoServicio _carritoServicio;
    private readonly PedidoServicio _pedidoServicio;

    public IndexModel(CarritoServicio carritoServicio, PedidoServicio pedidoServicio)
    {
        _carritoServicio = carritoServicio;
        _pedidoServicio = pedidoServicio;
    }

    public Cliente? Cliente { get; set; }
    public Domain.Carrito? Carrito { get; set; }
    public decimal Total { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!Request.Cookies.TryGetValue("IdClienteActivo", out string? idClienteStr) || !int.TryParse(idClienteStr, out int idCliente))
        {
            TempData["Mensaje"] = "Por favor identifícate o regístrate para continuar con la compra.";
            return RedirectToPage("/Clientes/Registrar");
        }

        Cliente = await _carritoServicio.ObtenerClienteAsync(idCliente);
        if (Cliente == null)
        {
            return RedirectToPage("/Clientes/Registrar");
        }

        Carrito = await _carritoServicio.ObtenerCarritoPorClienteAsync(idCliente);
        if (Carrito == null || !Carrito.Items.Any())
        {
            TempData["Mensaje"] = "Tu carrito está vacío. Agrega productos antes de realizar el checkout.";
            return RedirectToPage("/Carrito/Index");
        }

        Total = Carrito.Items.Sum(i => (i.Variante?.Producto?.Precio ?? 0) * i.Cantidad);

        return Page();
    }

    public async Task<IActionResult> OnPostConfirmarAsync()
    {
        if (!Request.Cookies.TryGetValue("IdClienteActivo", out string? idClienteStr) || !int.TryParse(idClienteStr, out int idCliente))
        {
            return RedirectToPage("/Clientes/Registrar");
        }

        var resultado = await _pedidoServicio.CrearPedidoAsync(idCliente);

        if (!resultado.Exito || resultado.Pedido == null)
        {
            TempData["Error"] = resultado.Mensaje;
            return RedirectToPage("/Carrito/Index");
        }

        TempData["Mensaje"] = "¡Pedido realizado con éxito! Tu orden ha sido registrada.";
        return RedirectToPage("/Pedidos/Detalle", new { id = resultado.Pedido.IdPedido });
    }
}
