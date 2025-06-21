using inventory8.DatabaseContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agrega el contexto de base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Agrega servicios de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => "La API está corriendo correctamente en Azure");

app.MapControllers();

app.Run();
