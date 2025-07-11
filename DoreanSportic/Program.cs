using DoraanSportic.Application.Profiles;
using DoreanSportic.Application.Services.Implementations;
using DoreanSportic.Application.Services.Interfaces;
using DoreanSportic.Infrastructure.Data;
using DoreanSportic.Infrastructure.Repository.Implementations;
using DoreanSportic.Infrastructure.Repository.Interfaces;
using DoreanSportic.Web.Middleware;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Para poder utilizar TempData
//***********************
// Configurar D.I.
//Repository
builder.Services.AddTransient<IRepositoryCarrito, RepositoryCarrito>();
builder.Services.AddTransient<IRepositoryCarritoDetalle, RepositoryCarritoDetalle>();
builder.Services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddTransient<IRepositoryCliente, RepositoryCliente>();
builder.Services.AddTransient<IRepositoryEmpaque, RepositoryEmpaque>();
builder.Services.AddTransient<IRepositoryEtiqueta, RepositoryEtiqueta>();
builder.Services.AddTransient<IRepositoryImagenProducto, RepositoryImagenProducto>();
builder.Services.AddTransient<IRepositoryMarca, RepositoryMarca>();
builder.Services.AddTransient<IRepositoryMetodoPago, RepositoryMetodoPago>();
builder.Services.AddTransient<IRepositoryPedido, RepositoryPedido>();
builder.Services.AddTransient<IRepositoryPedidoDetalle, RepositoryPedidoDetalle>();
builder.Services.AddTransient<IRepositoryProducto, RepositoryProducto>();
builder.Services.AddTransient<IRepositoryPromocion, RepositoryPromocion>();
builder.Services.AddTransient<IRepositoryResennaValoracion, RepositoryResennaValoracion>();
builder.Services.AddTransient<IRepositoryRol, RepositoryRol>();
builder.Services.AddTransient<IRepositorySexo, RepositorySexo>();
builder.Services.AddTransient<IRepositoryTarjeta, RepositoryTarjeta>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();

//Services
builder.Services.AddTransient<IServiceCarrito, ServiceCarrito>();
builder.Services.AddTransient<IServiceCarritoDetalle, ServiceCarritoDetalle>();
builder.Services.AddTransient<IServiceCategoria, ServiceCategoria>();
builder.Services.AddTransient<IServiceCliente, ServiceCliente>();
builder.Services.AddTransient<IServiceEtiqueta, ServiceEtiqueta>();
builder.Services.AddTransient<IServiceEmpaque, ServiceEmpaque>();
builder.Services.AddTransient<IServiceImagenProducto, ServiceImagenProducto>();
builder.Services.AddTransient<IServiceMarca, ServiceMarca>();
builder.Services.AddTransient<IServiceMetodoPago, ServiceMetodoPago>();
builder.Services.AddTransient<IServicePedido, ServicePedido>();
builder.Services.AddTransient<IServicePedidoDetalle, ServicePedidoDetalle>();
builder.Services.AddTransient<IServiceProducto, ServiceProducto>();
builder.Services.AddTransient<IServicePromocion, ServicePromocion>();
builder.Services.AddTransient<IServiceResennaValoracion, ServiceResennaValoracion>();
builder.Services.AddTransient<IServiceRol, ServiceRol>();
builder.Services.AddTransient<IServiceSexo, ServiceSexo>();
builder.Services.AddTransient<IServiceTarjeta, ServiceTarjeta>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();

//Configurar Automapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<CarritoProfile>();
    config.AddProfile<CarritoDetalleProfile>();
    config.AddProfile<CategoriaProfile>();
    config.AddProfile<ClienteProfile>();
    config.AddProfile<EtiquetaProfile>();
    config.AddProfile<EmpaqueProfile>();
    config.AddProfile<ImagenProductoProfile>();
    config.AddProfile<MarcaProfile>();
    config.AddProfile<MetodoPagoProfile>();
    config.AddProfile<PedidoProfile>();
    config.AddProfile<PedidoDetalleProfile>();
    config.AddProfile<ProductoProfile>();
    config.AddProfile<PromocionProfile>();
    config.AddProfile<ResennaValoracionProfile>();
    config.AddProfile<RolProfile>();
    config.AddProfile<SexoProfile>();
    config.AddProfile<TarjetaProfile>();
    config.AddProfile<UsuarioProfile>();
});

// Configuar Conexión a la Base de Datos SQL
builder.Services.AddDbContext<DoreanSporticContext>(options =>
{
    // it read appsettings.json file
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

//***********************
//Configuración Serilog
// Logger. P.E. Verbose = muestra SQl Statement
var logger = new LoggerConfiguration()
                    // Limitar la información de depuración
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    // Log LogEventLevel.Verbose muestra mucha información, pero no es necesaria solo para el proceso de depuración
                    .WriteTo.Console(LogEventLevel.Information)
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .CreateLogger();

builder.Host.UseSerilog(logger);
//***************************

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Error control Middleware
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

//Activar soporte a la solicitud de registro con SERILOG
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();//Para utilizar TempData

app.UseAuthorization();

// Activar Antiforgery 
app.UseAntiforgery();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//Prueba
app.Run();
