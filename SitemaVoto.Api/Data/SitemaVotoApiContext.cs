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

      
    }
