using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TiendaRopa.Infraestructura;

namespace TiendaRopa.Pages.Admin.Configuracion;

public class IndexModel : PageModel
{
    private readonly TiendaRopaContext _context;

    public IndexModel(TiendaRopaContext context)
    {
        _context = context;
    }

    public string NombreTienda { get; set; } = "TiendaRopa";
    public string Descripcion { get; set; } = "Tienda de moda y confección de prendas de vestir de alta calidad.";
    public string Correo { get; set; } = "contacto@tiendaropa.com";
    public string Telefono { get; set; } = "+51 987 654 321";
    public string Direccion { get; set; } = "Av. La Moda 123, Lima - Perú";
    public string Moneda { get; set; } = "PEN (Soles - S/)";
    public string BaseDeDatos { get; set; } = "TIENDA_ROPA_DB (SQL Server)";
    public bool EstadoBD { get; set; } = true;

    public async Task OnGetAsync()
    {
        try
        {
            EstadoBD = await _context.Database.CanConnectAsync();
        }
        catch
        {
            EstadoBD = false;
        }
    }
}
