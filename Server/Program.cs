using System.Text.Json.Serialization;
using FacturacionFfacsa.Server.Data;
using FacturacionFfacsa.Server.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración esencial de servicios
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 32; // Profundidad máxima
    });

builder.Services.AddRazorPages();

// 2. Configuración de base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging() // Solo para desarrollo
           .LogTo(Console.WriteLine, LogLevel.Information); // Log de queries
});

// 3. Servicios personalizados
builder.Services.AddScoped<IFacturaService, FacturaService>();

// 4. Configuración CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("api/Factura") // 
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .AllowCredentials());
});

// 5. Configuración de logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configuración del pipeline HTTP

// 6. Manejo de errores mejorado
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseDeveloperExceptionPage(); // Detalles de errores en desarrollo
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 7. Middlewares esenciales
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// 8. CORS debe estar antes de UseRouting
app.UseCors("AllowSpecificOrigin");

app.UseRouting();

// 9. Configuración de endpoints
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

// 10. Middleware para loggear todas las solicitudes
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.Run();