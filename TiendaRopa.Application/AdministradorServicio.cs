using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaRopa.Domain;
using TiendaRopa.Infraestructura;

namespace TiendaRopa.Application;

public class AdministradorServicio
{
    private readonly TiendaRopaContext _context;
    private readonly PasswordHasher<Administrador> _passwordHasher;

    public AdministradorServicio(TiendaRopaContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Administrador>();
    }

    public async Task<(bool Exito, Administrador? Admin, string Mensaje)> ValidarCredencialesAsync(string usuarioOCorreo, string password)
    {
        if (string.IsNullOrWhiteSpace(usuarioOCorreo) || string.IsNullOrWhiteSpace(password))
            return (false, null, "Debe ingresar usuario/correo y contraseña.");

        var term = usuarioOCorreo.Trim().ToLower();

        var admin = await _context.Administradores
            .FirstOrDefaultAsync(a => a.Usuario.ToLower() == term || a.Correo.ToLower() == term);

        if (admin == null)
            return (false, null, "Credenciales incorrectas.");

        if (!admin.Activo)
            return (false, null, "Esta cuenta de administrador se encuentra inactiva.");

        var verificacion = _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
        if (verificacion == PasswordVerificationResult.Failed)
            return (false, null, "Credenciales incorrectas.");

        return (true, admin, "Acceso concedido.");
    }

    public async Task AsegurarAdminInicialAsync()
    {
        if (!await _context.Administradores.AnyAsync())
        {
            var adminInicial = new Administrador
            {
                Nombre = "Administrador Principal",
                Usuario = "admin",
                Correo = "admin@tiendaropa.com",
                Rol = "Administrador",
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            adminInicial.PasswordHash = _passwordHasher.HashPassword(adminInicial, "Admin123*");

            _context.Administradores.Add(adminInicial);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Administrador?> ObtenerPorIdAsync(int id)
    {
        return await _context.Administradores.FindAsync(id);
    }

    public async Task<List<Administrador>> ObtenerTodosAsync()
    {
        return await _context.Administradores.OrderBy(a => a.Nombre).ToListAsync();
    }
}
