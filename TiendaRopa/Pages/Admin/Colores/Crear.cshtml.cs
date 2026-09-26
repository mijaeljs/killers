using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Colores;

public class CrearModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public CrearModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    [Required(ErrorMessage = "El nombre del color es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var (exito, mensaje) = await _catalogoAdmin.CrearColorAsync(Nombre);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
            return RedirectToPage("Index");
        }

        ModelState.AddModelError(string.Empty, mensaje);
        return Page();
    }
}
