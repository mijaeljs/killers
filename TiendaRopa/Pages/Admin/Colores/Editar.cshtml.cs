using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Colores;

public class EditarModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public EditarModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    public int IdColor { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "El nombre del color es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var color = await _catalogoAdmin.ObtenerColorPorIdAsync(id);
        if (color == null)
        {
            TempData["Error"] = "Color no encontrado.";
            return RedirectToPage("Index");
        }

        IdColor = color.IdColor;
        Nombre = color.Nombre;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var (exito, mensaje) = await _catalogoAdmin.ActualizarColorAsync(IdColor, Nombre);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
            return RedirectToPage("Index");
        }

        ModelState.AddModelError(string.Empty, mensaje);
        return Page();
    }
}
