using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Productos;

public class CrearModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public CrearModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [BindProperty]
    public string Descripcion { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 100000, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar una categoría.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
    public int IdCategoria { get; set; }

    public SelectList Categorias { get; set; } = default!;

    public async Task OnGetAsync()
    {
        await CargarCategoriasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        var (exito, mensaje, idProducto) = await _catalogoAdmin.CrearProductoAsync(Nombre, Descripcion, Precio, IdCategoria);
        if (exito)
        {
            TempData["Mensaje"] = $"{mensaje} Ahora puede agregarle las variantes de Talla y Color.";
            return RedirectToPage("/Admin/Variantes/Crear", new { idProducto });
        }

        ModelState.AddModelError(string.Empty, mensaje);
        await CargarCategoriasAsync();
        return Page();
    }

    private async Task CargarCategoriasAsync()
    {
        var categorias = await _catalogoAdmin.ListarCategoriasAsync();
        Categorias = new SelectList(categorias, "IdCategoria", "Nombre", IdCategoria);
    }
}
