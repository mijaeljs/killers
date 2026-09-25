using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Pedidos.Admin;

public class IndexModel : PageModel
{
    private readonly PedidoServicio _pedidoServicio;

    public IndexModel(PedidoServicio pedidoServicio)
    {
        _pedidoServicio = pedidoServicio;
    }

    public List<Pedido> Pedidos { get; set; } = new();

    public async Task OnGetAsync()
    {
        Pedidos = await _pedidoServicio.ObtenerTodosLosPedidosAsync();
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

        return RedirectToPage();
    }
}
