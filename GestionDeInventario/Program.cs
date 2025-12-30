using GestionDeInventario.Data;
using GestionDeInventario.Repository.Implementations;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Implementations;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Conn");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});
// Registro de la Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // ➡️ Ruta a la que se redirige si el usuario no está logueado
        options.LoginPath = "/Auth/Login";

        // ➡️ Ruta a la que se redirige si el usuario no tiene permisos (ej: rol "Admin" requerido)
        options.AccessDeniedPath = "/Home/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Duración de la sesión
    });


// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

//// Añadir soporte para Controladores con Vistas
//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(options =>
{
    // Crear una política que requiere que el usuario esté autenticado
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Aplicar la política de autorización a *todos* los controladores y acciones
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IDetalleCompraRepository, DetalleCompraRepository>();
builder.Services.AddScoped<IDetalleCompraService, DetalleCompraService>();
builder.Services.AddScoped<IDetalleDistribucionRepository, DetalleDistribucionRepository>();
builder.Services.AddScoped<IDetalleDistribucionService, DetalleDistribucionService>();


var app = builder.Build();

// Configuración de Middleware de Errores y Entorno
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles(); // Para archivos CSS, JS, imágenes

app.UseStaticFiles();
app.UseRouting();

// ➡️ Habilitar Autenticación y Autorización
// Debe ir *después* de UseRouting y *antes* de UseEndpoints/MapControllerRoute
app.UseAuthentication(); // Lee la cookie y establece el HttpContext.User
app.UseAuthorization();  // Aplica las políticas de Authorize (Roles, etc.)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
