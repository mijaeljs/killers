using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Admin.Productos;

public class EditarModel : PageModel
{
    private readonly CatalogoAdminServicio _catalogoAdmin;

    public EditarModel(CatalogoAdminServicio catalogoAdmin)
    {
        _catalogoAdmin = catalogoAdmin;
    }

    [BindProperty]
    public int IdProducto { get; set; }

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

    public List<ProductoVariante> Variantes { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var producto = await _catalogoAdmin.ObtenerProductoPorIdAsync(id);
        if (producto == null)
        {
            TempData["Error"] = "Producto no encontrado.";
            return RedirectToPage("Index");
        }

        IdProducto = producto.IdProducto;
        Nombre = producto.Nombre;
        Descripcion = producto.Descripcion;
        Precio = producto.Precio;
        IdCategoria = producto.IdCategoria;
        Variantes = producto.Variantes.ToList();

        await CargarCategoriasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            var prod = await _catalogoAdmin.ObtenerProductoPorIdAsync(IdProducto);
            if (prod != null) Variantes = prod.Variantes.ToList();
            return Page();
        }

        var (exito, mensaje) = await _catalogoAdmin.ActualizarProductoAsync(IdProducto, Nombre, Descripcion, Precio, IdCategoria);
        if (exito)
        {
            TempData["Mensaje"] = mensaje;
            return RedirectToPage("Index");
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
