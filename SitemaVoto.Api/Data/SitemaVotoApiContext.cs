using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VotoModelos;
using VotoModelos.Entidades;

    public class SitemaVotoApiContext : DbContext
    {
        public SitemaVotoApiContext (DbContextOptions<SitemaVotoApiContext> options)
            : base(options)
        {
        }

public DbSet<VotoModelos.Entidades.Usuario> Usuarios { get; set; } = default!;

public DbSet<VotoModelos.Entidades.PerfilVotante> PerfilVotantes { get; set; } = default!;

public DbSet<VotoModelos.Entidades.ProcesoElectoral> ProcesoElectorales { get; set; } = default!;

public DbSet<VotoModelos.Entidades.Candidato> Candidatos { get; set; } = default!;

public DbSet<VotoModelos.Entidades.CodigoPadron> CodigoPadrones { get; set; } = default!;

public DbSet<VotoModelos.Entidades.ParticipacionVotante> ParticipacionVotantes { get; set; } = default!;

public DbSet<VotoModelos.Entidades.Voto> Votos { get; set; } = default!;

public DbSet<VotoModelos.Entidades.Auditoria> Auditorias { get; set; } = default!;

    public DbSet<Junta> Juntas { get; set; } = default!;

    // =========================
    // RELACIONES
    // =========================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // JUNTA -> JEFE (Usuario)
        // =========================
        modelBuilder.Entity<Junta>()
            .HasOne(j => j.JefeJuntaUsuario)
            .WithMany() // sin navegación en Usuario
            .HasForeignKey(j => j.JefeJuntaUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        // =========================
        // USUARIO -> JUNTA (votantes)
        // =========================
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Junta)
            .WithMany(j => j.Usuarios)
            .HasForeignKey(u => u.JuntaId)
            .OnDelete(DeleteBehavior.SetNull);

        // =========================
        // OTP
        // =========================
        modelBuilder.Entity<OtpSesion>()
            .HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // CODIGO PADRON (2 FK a Usuario)
        // =========================
        modelBuilder.Entity<CodigoPadron>()
            .HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CodigoPadron>()
            .HasOne(x => x.EmitidoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.EmitidoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CodigoPadron>()
            .HasIndex(x => new { x.UsuarioId, x.ProcesoElectoralId })
            .IsUnique();

        // =========================
        // PARTICIPACION (voto único)
        // =========================
        modelBuilder.Entity<ParticipacionVotante>()
            .HasIndex(x => new { x.UsuarioId, x.ProcesoElectoralId })
            .IsUnique();

        // =========================
        // VOTO (secreto)
        // =========================
        modelBuilder.Entity<Voto>()
            .HasOne(x => x.Candidato)
            .WithMany()
            .HasForeignKey(x => x.CandidatoId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public DbSet<VotoModelos.Entidades.OtpSesion> OtpSesiones { get; set; } = default!;
}
