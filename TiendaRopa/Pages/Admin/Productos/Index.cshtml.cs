using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Productos;

public class IndexModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public IndexModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    public List<Producto> Productos { get; set; } = new();

    public SelectList CategoriasFiltro { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? Buscar { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? IdCategoria { get; set; }

    public async Task OnGetAsync()
    {
        var categorias = await _catalogoAdmin.ListarCategoriasAsync();
        CategoriasFiltro = new SelectList(categorias, "IdCategoria", "Nombre", IdCategoria);

        Productos = await _catalogoAdmin.ListarProductosAsync(Buscar, IdCategoria);
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var (exito, mensaje) = await _catalogoAdmin.EliminarProductoAsync(id);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
        }
        else
        {
            TempData["Error"] = mensaje;
        }

        return RedirectToPage(new { Buscar, IdCategoria });
    }
}
