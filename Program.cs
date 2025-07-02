using inventory8.DatabaseContext;
using inventory8.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

/*Section: Clever Cloud oAuth*/
// Configura autenticación
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "CleverCloud";
})
.AddCookie()
.AddOAuth("CleverCloud", options =>
{
    options.ClientId = builder.Configuration["CleverCloud:ClientId"]; // Mejor en appsettings.json o secrets
    options.ClientSecret = builder.Configuration["CleverCloud:ClientSecret"];
    options.CallbackPath = new PathString("/signin-clevercloud");

    options.AuthorizationEndpoint = "https://www.clever-cloud.com/oauth/authorize";
    options.TokenEndpoint = "https://www.clever-cloud.com/oauth/token";
    options.UserInformationEndpoint = "https://api.clever-cloud.com/v2/self";

    options.Scope.Add("user:read");

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");

    options.SaveTokens = true;

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error al obtener información del usuario de Clever Cloud");
            }

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonDocument.Parse(json);

            context.RunClaimActions(user.RootElement);
        }
    };
});
/**/
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
