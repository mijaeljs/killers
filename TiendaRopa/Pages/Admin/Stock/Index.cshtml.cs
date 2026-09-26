using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Stock;

public class IndexModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public IndexModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    public List<ProductoVariante> Variantes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Buscar { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? EstadoFiltro { get; set; }

    public int TotalPrendas { get; set; }
    public int CantidadStockBajo { get; set; }
    public int CantidadSinStock { get; set; }
    public int TotalUnidadesEnAlmacen { get; set; }

    public async Task OnGetAsync()
    {
        var todas = await _catalogoAdmin.ListarVariantesAsync(null, Buscar);

        TotalPrendas = todas.Count;
        TotalUnidadesEnAlmacen = todas.Sum(v => v.Stock);
        CantidadStockBajo = todas.Count(v => v.Stock > 0 && v.Stock <= 5);
        CantidadSinStock = todas.Count(v => v.Stock == 0);

        if (EstadoFiltro == "bajo")
        {
            Variantes = todas.Where(v => v.Stock > 0 && v.Stock <= 5).ToList();
        }
        else if (EstadoFiltro == "agotado")
        {
            Variantes = todas.Where(v => v.Stock == 0).ToList();
        }
        else if (EstadoFiltro == "normal")
        {
            Variantes = todas.Where(v => v.Stock > 5).ToList();
        }
        else
        {
            Variantes = todas;
        }
    }

    public async Task<IActionResult> OnPostActualizarStockAsync(int idVariante, int nuevoStock)
    {
        var (exito, mensaje) = await _catalogoAdmin.ActualizarStockAsync(idVariante, nuevoStock);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
        }
        else
        {
            TempData["Error"] = mensaje;
        }

        return RedirectToPage(new { Buscar, EstadoFiltro });
    }
}
