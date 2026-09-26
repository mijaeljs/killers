using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Colores;

public class IndexModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public IndexModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    public List<Color> Colores { get; set; } = new();

    public async Task OnGetAsync()
    {
        Colores = await _catalogoAdmin.ListarColoresAsync();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var (exito, mensaje) = await _catalogoAdmin.EliminarColorAsync(id);
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
