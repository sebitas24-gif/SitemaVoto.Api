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

            public DbSet<VotoModelos.Auditoria> Auditorias { get; set; } = default!;

    public DbSet<VotoModelos.Papeleta> Papeletas { get; set; } = default!;

    public DbSet<VotoModelos.ProcesoElectoral> ProcesoElectorals { get; set; } = default!;

    public DbSet<VotoModelos.Votante> Votantes { get; set; } = default!;

    public DbSet<VotoModelos.Voto> Votos { get; set; } = default!;

    public DbSet<VotoModelos.OpcionElectoral> OpcionElectorales { get; set; } = default!;
    }
