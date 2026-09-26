using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Clientes;

public class IndexModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public IndexModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    public List<ClienteAdminDto> Clientes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Buscar { get; set; }

    public async Task OnGetAsync()
    {
        Clientes = await _catalogoAdmin.ListarClientesConEstadisticasAsync(Buscar);
    }
}
