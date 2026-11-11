using System.Text;
using ApiClinica.Data;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using ApiClinica.Models;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Adiciona o banco (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=clinica.db"));

// ---------- Injeção de Dependências ----------
// Mappers: injetados nos Services.
builder.Services.AddScoped<IPacienteMapper, PacienteMapper>();
builder.Services.AddScoped<IMedicoMapper, MedicoMapper>();
builder.Services.AddScoped<IConsultaMapper, ConsultaMapper>();

// Services: injetados nos Controllers.
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

// Autenticação: hash de senha, geração de token e regras de login/registro.
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ---------- Autenticação JWT ----------
var jwt = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ---------- Autorização ----------
// Estratégia: autorização baseada em papéis (roles). O token carrega a role
// "Comum" ou "Admin", e os métodos PATCH/DELETE usam [Authorize(Roles = "Admin")].
builder.Services.AddAuthorization();

var app = builder.Build();

// Aplica as migrations e cria o Admin inicial, se ainda não existir.
await SeedAdmin.ExecutarAsync(app.Services);

app.UseHttpsRedirection();

app.UseAuthentication(); // Quem é você? (lê e valida o token)
app.UseAuthorization();  // O que você pode fazer? (checa [Authorize] e roles)

app.MapGet("/alive", () => Results.Ok(new
{
    status = "alive",
    utc = DateTime.UtcNow
}));

app.MapControllers();

app.Run();
