using Microsoft.EntityFrameworkCore;
using TiendaScaffolding.Data;

// La interfaz utiliza punto como separador decimal (HTML input type=number).
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

// Obtener conexion MySQL
var conexion = builder.Configuration
    .GetConnectionString("TiendaMySql")
    ?? throw new InvalidOperationException("Falta la cadena TiendaMySql en appsettings.json.");

// Registrar el contexto de base de datos
builder.Services.AddDbContext<TiendaDbContext>(
    options => options.UseMySql(
        conexion,
        ServerVersion.AutoDetect(conexion)
    )
);

var app = builder.Build();

// Configuracion de la aplicacion
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Ruta principal
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
