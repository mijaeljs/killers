using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Pedidos;

public class IndexModel : PageModel
{
    private readonly PedidoServicio _pedidoServicio;

    public IndexModel(PedidoServicio pedidoServicio)
    {
        _pedidoServicio = pedidoServicio;
    }

    public List<Pedido> Pedidos { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? EstadoFiltro { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Buscar { get; set; }

    public int TotalPedidos { get; set; }
    public decimal TotalVentas { get; set; }

    public async Task OnGetAsync()
    {
        var todos = await _pedidoServicio.ObtenerTodosLosPedidosAsync();

        TotalPedidos = todos.Count;
        TotalVentas = todos.Where(p => p.Estado != "Cancelado").Sum(p => p.Total);

        if (!string.IsNullOrWhiteSpace(Buscar))
        {
            var term = Buscar.Trim().ToLower();
            todos = todos.Where(p =>
                p.IdPedido.ToString().Contains(term) ||
                (p.Cliente != null && (p.Cliente.Nombre.ToLower().Contains(term) || p.Cliente.Correo.ToLower().Contains(term)))
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(EstadoFiltro))
        {
            todos = todos.Where(p => p.Estado.Equals(EstadoFiltro, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        Pedidos = todos;
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

        return RedirectToPage(new { EstadoFiltro, Buscar });
    }
}
