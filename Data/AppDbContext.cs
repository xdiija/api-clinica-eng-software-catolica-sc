using ApiClinica.Models;
using ApiClinica.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Consulta> Consultas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var paciente = modelBuilder.Entity<Paciente>();

        paciente.Property(p => p.Nome).HasMaxLength(120);

        paciente.Property(p => p.Email)
            .HasConversion(email => email.Valor, valor => Email.Reconstituir(valor))
            .HasMaxLength(254);

        paciente.Property(p => p.Telefone)
            .HasConversion(tel => tel.Valor, valor => Telefone.Reconstituir(valor))
            .HasMaxLength(16);

        paciente.Property(p => p.DataNasc)
            .HasConversion(data => data.Valor, valor => DataNascimento.Reconstituir(valor));

        paciente.Property(p => p.Cpf)
            .HasConversion(cpf => cpf.Valor, valor => Cpf.Reconstituir(valor))
            .HasMaxLength(11);

        paciente.HasIndex(p => p.Cpf).IsUnique();

        var medico = modelBuilder.Entity<Medico>();

        medico.Property(m => m.Nome).HasMaxLength(120);

        medico.Property(m => m.Email)
            .HasConversion(email => email.Valor, valor => Email.Reconstituir(valor))
            .HasMaxLength(254);

        medico.Property(m => m.Telefone)
            .HasConversion(tel => tel.Valor, valor => Telefone.Reconstituir(valor))
            .HasMaxLength(16);

        medico.Property(m => m.CRM).HasMaxLength(20);

        var consulta = modelBuilder.Entity<Consulta>();

        consulta.Property(c => c.DataHora)
            .HasConversion(janela => janela.Inicio, inicio => JanelaConsulta.De(inicio));

        consulta.HasOne(c => c.Paciente)
            .WithMany(p => p.Consultas)
            .HasForeignKey(c => c.PacienteId)
            .OnDelete(DeleteBehavior.Cascade);

        consulta.HasOne(c => c.Medico)
            .WithMany(m => m.Consultas)
            .HasForeignKey(c => c.MedicoId)
            .OnDelete(DeleteBehavior.Cascade);

        consulta.HasIndex(c => new { c.MedicoId, c.DataHora });
        consulta.HasIndex(c => new { c.PacienteId, c.DataHora });
    }
}
