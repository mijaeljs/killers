using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Clientes;

public class DetalleModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;
    private readonly PedidoServicio _pedidoServicio;

    public DetalleModel(CatalogoAdminServicio catalogoAdmin, PedidoServicio pedidoServicio)
    {
        _catalogoAdmin = catalogoAdmin;
        _pedidoServicio = pedidoServicio;
    }

    public Cliente? Cliente { get; set; }
    public List<Pedido> Pedidos { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Cliente = await _catalogoAdmin.ObtenerDetalleClienteAsync(id);
        if (Cliente == null)
        {
            TempData["Error"] = "Cliente no encontrado.";
            return RedirectToPage("Index");
        }

        Pedidos = await _pedidoServicio.ObtenerPedidosPorClienteAsync(id);
        return Page();
    }
}
