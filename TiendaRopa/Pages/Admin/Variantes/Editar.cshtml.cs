using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Variantes;

public class EditarModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public EditarModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    public int IdVariante { get; set; }

    public ProductoVariante? Variante { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar una talla.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una talla válida.")]
    public int IdTalla { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar un color.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un color válido.")]
    public int IdColor { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "El stock es obligatorio.")]
    [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    public SelectList Tallas { get; set; } = default!;
    public SelectList Colores { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Variante = await _catalogoAdmin.ObtenerVariantePorIdAsync(id);
        if (Variante == null)
        {
            TempData["Error"] = "Variante no encontrada.";
            return RedirectToPage("Index");
        }

        IdVariante = Variante.IdVariante;
        IdTalla = Variante.IdTalla;
        IdColor = Variante.IdColor;
        Stock = Variante.Stock;

        await CargarCombosAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Variante = await _catalogoAdmin.ObtenerVariantePorIdAsync(IdVariante);
            await CargarCombosAsync();
            return Page();
        }

        var (exito, mensaje) = await _catalogoAdmin.ActualizarVarianteAsync(IdVariante, IdTalla, IdColor, Stock);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
            return RedirectToPage("Index");
        }

        ModelState.AddModelError(string.Empty, mensaje);
        Variante = await _catalogoAdmin.ObtenerVariantePorIdAsync(IdVariante);
        await CargarCombosAsync();
        return Page();
    }

    private async Task CargarCombosAsync()
    {
        var tallas = await _catalogoAdmin.ListarTallasAsync();
        var colores = await _catalogoAdmin.ListarColoresAsync();

        Tallas = new SelectList(tallas, "IdTalla", "Nombre", IdTalla);
        Colores = new SelectList(colores, "IdColor", "Nombre", IdColor);
    }
}
