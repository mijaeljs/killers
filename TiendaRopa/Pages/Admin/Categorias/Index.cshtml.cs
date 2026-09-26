using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Categorias;

public class IndexModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public IndexModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    public List<Categoria> Categorias { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categorias = await _catalogoAdmin.ListarCategoriasAsync();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var (exito, mensaje) = await _catalogoAdmin.EliminarCategoriaAsync(id);
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
