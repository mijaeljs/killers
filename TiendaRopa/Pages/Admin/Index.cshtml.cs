using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly DashboardServicio _dashboardServicio;

    public IndexModel(DashboardServicio dashboardServicio)
    {
        _dashboardServicio = dashboardServicio;
    }

    public DashboardResumenDto Resumen { get; set; } = new();

    public async Task OnGetAsync()
    {
        Resumen = await _dashboardServicio.ObtenerResumenAsync();
    }
}
