using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Tallas;

public class IndexModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public IndexModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    public List<Talla> Tallas { get; set; } = new();

    public async Task OnGetAsync()
    {
        Tallas = await _catalogoAdmin.ListarTallasAsync();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var (exito, mensaje) = await _catalogoAdmin.EliminarTallaAsync(id);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
        }
        else
        {
            TempData["Error"] = mensaje;
        }

        return RedirectToPage();
    }
}
