using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Pedidos;

public class DetalleModel : PageModel
{
    private readonly PedidoServicio _pedidoServicio;

    public DetalleModel(PedidoServicio pedidoServicio)
    {
        _pedidoServicio = pedidoServicio;
    }

    public Pedido? Pedido { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!Request.Cookies.TryGetValue("IdClienteActivo", out string? idClienteStr) || !int.TryParse(idClienteStr, out int idCliente))
        {
            TempData["Mensaje"] = "Por favor identifícate o regístrate para ver el detalle de tus pedidos.";
            return RedirectToPage("/Clientes/Registrar");
        }

        Pedido = await _pedidoServicio.ObtenerDetallePedidoAsync(id, idCliente);

        if (Pedido == null)
        {
            TempData["Error"] = "El pedido solicitado no fue encontrado o no pertenece a tu cuenta.";
            return RedirectToPage("Index");
        }

        return Page();
    }
}
