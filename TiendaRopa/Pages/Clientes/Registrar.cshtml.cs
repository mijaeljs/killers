using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;
using TiendaRopa.Domain;

namespace TiendaRopa.Pages.Clientes;

public class RegistrarModel : PageModel
{
    private readonly CarritoServicio _carritoServicio;

    public RegistrarModel(CarritoServicio carritoServicio)
    {
        _carritoServicio = carritoServicio;
    }

    [BindProperty]
    public Cliente Cliente { get; set; } = new();

    public string? MensajeError { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var clienteCreado = await _carritoServicio.RegistrarClienteAsync(Cliente);

        // Guardamos temporalmente el IdCliente en la sesión / cookie rápida
        Response.Cookies.Append("IdClienteActivo", clienteCreado.IdCliente.ToString());

        return RedirectToPage("/Productos/Index");
    }
}