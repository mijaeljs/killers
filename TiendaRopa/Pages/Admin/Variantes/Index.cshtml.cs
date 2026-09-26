using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Variantes;

public class IndexModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public IndexModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    public List<ProductoVariante> Variantes { get; set; } = new();

    public SelectList ProductosFiltro { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int? IdProducto { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Buscar { get; set; }

    public async Task OnGetAsync()
    {
        var productos = await _catalogoAdmin.ListarProductosAsync();
        ProductosFiltro = new SelectList(productos, "IdProducto", "Nombre", IdProducto);

        Variantes = await _catalogoAdmin.ListarVariantesAsync(IdProducto, Buscar);
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var (exito, mensaje) = await _catalogoAdmin.EliminarVarianteAsync(id);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
        }
        else
        {
            TempData["Error"] = mensaje;
        }

        return RedirectToPage(new { IdProducto, Buscar });
    }
}
