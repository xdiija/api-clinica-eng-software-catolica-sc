using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Adiciona o banco (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=clinica.db"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/alive", () => Results.Ok(new
{
    status = "alive",
    utc = DateTime.UtcNow
}));

app.MapControllers();

app.Run();