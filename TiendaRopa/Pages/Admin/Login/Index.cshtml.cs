using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TiendaRopa.Application;

namespace TiendaRopa.Pages.Admin.Login;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly AdministradorServicio _adminServicio;

    public IndexModel(AdministradorServicio adminServicio)
    {
        _adminServicio = adminServicio;
    }

    [BindProperty]
    [Required(ErrorMessage = "Ingrese su usuario o correo electrónico.")]
    [Display(Name = "Usuario o correo")]
    public string UsuarioOCorreo { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool Recordarme { get; set; } = false;

    public string? ErrorMensaje { get; set; }

    public IActionResult OnGet(string? returnUrl = null)
    {
        // Si ya está autenticado, dirigirlo directo al Dashboard
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Admin/Index");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (exito, admin, mensaje) = await _adminServicio.ValidarCredencialesAsync(UsuarioOCorreo, Password);

        if (!exito || admin == null)
        {
            ErrorMensaje = mensaje;
            return Page();
        }

        // Crear los Claims del administrador autenticado
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.IdAdministrador.ToString()),
            new(ClaimTypes.Name, admin.Nombre),
            new(ClaimTypes.Email, admin.Correo),
            new("Usuario", admin.Usuario),
            new(ClaimTypes.Role, admin.Rol)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = Recordarme,
            ExpiresUtc = Recordarme ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToPage("/Admin/Index");
    }
}
