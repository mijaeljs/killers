using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Tallas;

public class EditarModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public EditarModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    public int IdTalla { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "El nombre de la talla es obligatorio.")]
    [MaxLength(20, ErrorMessage = "El nombre no puede superar los 20 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var talla = await _catalogoAdmin.ObtenerTallaPorIdAsync(id);
        if (talla == null)
        {
            TempData["Error"] = "Talla no encontrada.";
            return RedirectToPage("Index");
        }

        IdTalla = talla.IdTalla;
        Nombre = talla.Nombre;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var (exito, mensaje) = await _catalogoAdmin.ActualizarTallaAsync(IdTalla, Nombre);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
            return RedirectToPage("Index");
        }

        ModelState.AddModelError(string.Empty, mensaje);
        return Page();
    }
}
