using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Variantes;

public class CrearModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public CrearModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar un producto.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto válido.")]
    public int IdProducto { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar una talla.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una talla válida.")]
    public int IdTalla { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar un color.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un color válido.")]
    public int IdColor { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "El stock inicial es obligatorio.")]
    [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; } = 10;

    public SelectList Productos { get; set; } = default!;
    public SelectList Tallas { get; set; } = default!;
    public SelectList Colores { get; set; } = default!;

    public async Task OnGetAsync(int? idProducto = null)
    {
        if (idProducto.HasValue)
        {
            IdProducto = idProducto.Value;
        }

        await CargarCombosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarCombosAsync();
            return Page();
        }

        var (exito, mensaje) = await _catalogoAdmin.CrearVarianteAsync(IdProducto, IdTalla, IdColor, Stock);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
            return RedirectToPage("Index", new { idProducto = IdProducto });
        }

        ModelState.AddModelError(string.Empty, mensaje);
        await CargarCombosAsync();
        return Page();
    }

    private async Task CargarCombosAsync()
    {
        var productos = await _catalogoAdmin.ListarProductosAsync();
        var tallas = await _catalogoAdmin.ListarTallasAsync();
        var colores = await _catalogoAdmin.ListarColoresAsync();

        Productos = new SelectList(productos, "IdProducto", "Nombre", IdProducto);
        Tallas = new SelectList(tallas, "IdTalla", "Nombre", IdTalla);
        Colores = new SelectList(colores, "IdColor", "Nombre", IdColor);
    }
}
