using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TiendaRopa.Application;
using TiendaRopa.Infraestructura;

var builder = WebApplication.CreateBuilder(args);

// Configurar Razor Pages con autorización de carpeta /Admin
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AllowAnonymousToPage("/Admin/Login/Index");
});

builder.Services.AddDbContext<TiendaRopaContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Servicios de Negocio y Administración
builder.Services.AddScoped<ProductoServicio>();
builder.Services.AddScoped<CarritoServicio>();
builder.Services.AddScoped<PedidoServicio>();
builder.Services.AddScoped<AdministradorServicio>();
builder.Services.AddScoped<DashboardServicio>();
builder.Services.AddScoped<CatalogoAdminServicio>();

// Autenticación con Cookies para el Panel Administrativo
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.Cookie.Name = "TiendaRopa.AdminAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddSession();

var app = builder.Build();

// Asegurar existencia del administrador inicial en la BD
using (var scope = app.Services.CreateScope())
{
    var adminServicio = scope.ServiceProvider.GetRequiredService<AdministradorServicio>();
    await adminServicio.AsegurarAdminInicialAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();