using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VotoModelos;

    public class SitemaVotoApiContext : DbContext
    {
            public SitemaVotoApiContext (DbContextOptions<SitemaVotoApiContext> options)
                : base(options)
            {
            }
    public DbSet<VotoModelos.Papeleta> Papeletas { get; set; } = default!;

    public DbSet<VotoModelos.ProcesoElectoral> ProcesoElectorales { get; set; } = default!;

    public DbSet<VotoModelos.Votante> Votantes { get; set; } = default!;

    public DbSet<VotoModelos.Voto> Votos { get; set; } = default!;

    public DbSet<VotoModelos.OpcionElectoral> OpcionElectorales { get; set; } = default!;
  
    public DbSet<VotoModelos.Administrador> Administrador { get; set; } = default!;

    public DbSet<VotoModelos.Candidato> Candidato { get; set; } = default!;

    public DbSet<VotoModelos.ResultadoOpcion> ResultadoOpcion { get; set; } = default!;

    public DbSet<VotoModelos.HistorialResultados> HistorialResultados { get; set; } = default!;

















    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Votante (1) <-> (0..1) Administrador
        modelBuilder.Entity<Votante>()
            .HasOne(v => v.Administrador)
            .WithOne(a => a.Votante)
            .HasForeignKey<Administrador>(a => a.IdVotante)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Administrador>()
            .HasIndex(a => a.IdVotante)
            .IsUnique();

        // (Recomendado) también para Candidato, te saldrá luego:
        modelBuilder.Entity<Votante>()
            .HasOne(v => v.Candidato)
            .WithOne(c => c.Votante)
            .HasForeignKey<Candidato>(c => c.IdVotante)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Candidato>()
            .HasIndex(c => c.IdVotante)
            .IsUnique();

        // Y tu 1:1 de Historial, igual:
        modelBuilder.Entity<ProcesoElectoral>()
            .HasOne(p => p.Historial)
            .WithOne(h => h.ProcesoElectoral)
            .HasForeignKey<HistorialResultados>(h => h.IdProcesoElectoral)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HistorialResultados>()
            .HasIndex(h => h.IdProcesoElectoral)
            .IsUnique();
    }

}
