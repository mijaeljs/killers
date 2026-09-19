using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Productos;

public class CrearModel : PageModel
{
    private readonly ProductoServicio _productoServicio;

    public CrearModel(ProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    [BindProperty]
    public string Nombre { get; set; } = "";

    [BindProperty]
    public string Descripcion { get; set; } = "";

    [BindProperty]
    public decimal Precio { get; set; }

    [BindProperty]
    public int IdCategoria { get; set; }

    public SelectList Categorias { get; set; } = default!;

    public string? Mensaje { get; set; }

    public void OnGet()
    {
        CargarCategorias();
    }

    public IActionResult OnPost()
    {
        string resultado = _productoServicio.RegistrarProducto(Nombre, Descripcion, Precio, IdCategoria);

        if (resultado == "Producto registrado correctamente.")
        {
            return RedirectToPage("Index");
        }

        Mensaje = resultado;
        CargarCategorias();
        return Page();
    }

    private void CargarCategorias()
    {
        Categorias = new SelectList(_productoServicio.ObtenerCategorias(), "IdCategoria", "Nombre");
    }
}