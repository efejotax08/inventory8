using inventory8.DatabaseContext;
using inventory8.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
/*Section:JWT*/
// Clave secreta (usa un secreto fuerte en producción)
var claveSecreta = builder.Configuration["Jwt:Key"] ?? "clave-super-secreta-segura";
var emisor = builder.Configuration["Jwt:Issuer"] ?? "tu-backend";
var audiencia = builder.Configuration["Jwt:Audience"] ?? "tu-frontend";

// Configurar autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // true en producción
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = emisor,
            ValidAudience = audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
/**/


/*Section: Clever Cloud oAuth*/
// Configura autenticación y cache
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
/**/
//Notificaciones
//builder.Services.AddScoped<UserService>();
builder.Services.AddHostedService<InventoryMonitorService>();
builder.Services.AddScoped<InventoryAlertService>();
builder.Services.AddSingleton<UserNotificationService>();

// Configura la variable de entorno con la ruta del archivo JSON
var credentialsPath = Path.Combine(AppContext.BaseDirectory, "Credentials", "t-gateway-464020-n0-c2f9ef363c39.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

// Agrega servicios
builder.Services.AddSingleton<GoogleCloudStorageService>();
builder.Services.AddControllersWithViews();


// Agrega el contexto de base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// Add services to the container.

builder.Services.AddControllers();

// Agrega servicios de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseStaticFiles();
// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
    c.RoutePrefix = "swagger";
});


app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Bienvenido !");

app.MapControllers();

app.Run();
