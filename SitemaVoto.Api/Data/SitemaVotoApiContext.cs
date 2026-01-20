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

        // Usuario (1) -> (N) OTP
        modelBuilder.Entity<OtpSesion>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Junta>()
       .HasOne(j => j.JefeJuntaUsuario)
       .WithMany()
       .HasForeignKey(j => j.JefeJuntaUsuarioId)
       .OnDelete(DeleteBehavior.SetNull);
        // Usuario (1) -> (1) PerfilVotante (opcional)
        modelBuilder.Entity<PerfilVotante>()
            .HasOne<Usuario>()
            .WithOne()
            .HasForeignKey<PerfilVotante>(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProcesoElectoral (1) -> (N) Candidatos
        modelBuilder.Entity<Candidato>()
            .HasOne<ProcesoElectoral>()
            .WithMany()
            .HasForeignKey(x => x.ProcesoElectoralId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProcesoElectoral (1) -> (N) Votos
        modelBuilder.Entity<Voto>()
            .HasOne<ProcesoElectoral>()
            .WithMany()
            .HasForeignKey(x => x.ProcesoElectoralId)
            .OnDelete(DeleteBehavior.Cascade);

        // Candidato (1) -> (N) Votos
        // (nullable si es voto en blanco)
        modelBuilder.Entity<Voto>()
            .HasOne<Candidato>()
            .WithMany()
            .HasForeignKey(x => x.CandidatoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Usuario (1) -> (N) Participaciones
        modelBuilder.Entity<ParticipacionVotante>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Proceso (1) -> (N) Participaciones
        modelBuilder.Entity<ParticipacionVotante>()
            .HasOne<ProcesoElectoral>()
            .WithMany()
            .HasForeignKey(x => x.ProcesoElectoralId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Un usuario solo puede tener UNA participación por proceso
        modelBuilder.Entity<ParticipacionVotante>()
            .HasIndex(x => new { x.UsuarioId, x.ProcesoElectoralId })
            .IsUnique();

        // Usuario (1) -> (N) CodigosPadron
        modelBuilder.Entity<CodigoPadron>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Proceso (1) -> (N) CodigosPadron
        modelBuilder.Entity<CodigoPadron>()
            .HasOne<ProcesoElectoral>()
            .WithMany()
            .HasForeignKey(x => x.ProcesoElectoralId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Un usuario solo tiene UN código por proceso
        modelBuilder.Entity<CodigoPadron>()
            .HasIndex(x => new { x.UsuarioId, x.ProcesoElectoralId })
            .IsUnique();

        // Junta (1) -> (N) Usuarios (si tienes JuntaId en Usuario)
        modelBuilder.Entity<Usuario>()
        .HasOne(u => u.Junta)
        .WithMany(j => j.Usuarios)
        .HasForeignKey(u => u.JuntaId)
        .OnDelete(DeleteBehavior.SetNull);
    }

public DbSet<VotoModelos.Entidades.OtpSesion> OtpSesiones { get; set; } = default!;
}
