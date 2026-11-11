using ApiClinica.Models;
using ApiClinica.Models.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Data;

// Cria o usuário Admin inicial (lido do appsettings.json, seção "AdminInicial")
// caso ainda não exista nenhum Admin no banco. Assim o registro público só cria
// usuários comuns, e ninguém consegue se autopromover a Admin pela API.
public static class SeedAdmin
{
    public static async Task ExecutarAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>().GetSection("AdminInicial");

        await context.Database.MigrateAsync();

        if (await context.Usuarios.AnyAsync(u => u.Perfil == Perfis.Admin)) return;

        if (!Email.TryParse(config["Email"], out var email) || string.IsNullOrEmpty(config["Senha"]))
            return;

        var admin = new Usuario
        {
            Nome = config["Nome"] ?? "Administrador",
            Email = email,
            Perfil = Perfis.Admin
        };
        admin.SenhaHash = hasher.HashPassword(admin, config["Senha"]!);

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}
