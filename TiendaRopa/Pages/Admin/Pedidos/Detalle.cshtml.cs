using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Pedidos;

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
        Pedido = await _pedidoServicio.ObtenerDetallePedidoAsync(id);
        if (Pedido == null)
        {
            TempData["Error"] = "El pedido solicitado no existe.";
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCambiarEstadoAsync(int idPedido, string nuevoEstado)
    {
        var exito = await _pedidoServicio.CambiarEstadoAsync(idPedido, nuevoEstado);
        if (exito)
        {
            TempData["Mensaje"] = $"Estado del pedido #{idPedido.ToString("D4")} actualizado a '{nuevoEstado}'.";
        }
        else
        {
            TempData["Error"] = "No se pudo actualizar el estado del pedido.";
        }

        return RedirectToPage(new { id = idPedido });
    }
}
