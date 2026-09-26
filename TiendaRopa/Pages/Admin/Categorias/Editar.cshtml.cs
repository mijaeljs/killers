using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Categorias;

public class EditarModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public EditarModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    public int IdCategoria { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var categoria = await _catalogoAdmin.ObtenerCategoriaPorIdAsync(id);
        if (categoria == null)
        {
            TempData["Error"] = "Categoría no encontrada.";
            return RedirectToPage("Index");
        }

        IdCategoria = categoria.IdCategoria;
        Nombre = categoria.Nombre;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var (exito, mensaje) = await _catalogoAdmin.ActualizarCategoriaAsync(IdCategoria, Nombre);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
            return RedirectToPage("Index");
        }

        ModelState.AddModelError(string.Empty, mensaje);
        return Page();
    }
}
