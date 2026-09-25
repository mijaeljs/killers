using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Pedidos;

public class IndexModel : PageModel
{
    private readonly PedidoServicio _pedidoServicio;

    public IndexModel(PedidoServicio pedidoServicio)
    {
        _pedidoServicio = pedidoServicio;
    }

    public List<Pedido> Pedidos { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!Request.Cookies.TryGetValue("IdClienteActivo", out string? idClienteStr) || !int.TryParse(idClienteStr, out int idCliente))
        {
            TempData["Mensaje"] = "Por favor identifícate o regístrate para ver tus pedidos.";
            return RedirectToPage("/Clientes/Registrar");
        }

        Pedidos = await _pedidoServicio.ObtenerPedidosPorClienteAsync(idCliente);
        return Page();
    }
}
