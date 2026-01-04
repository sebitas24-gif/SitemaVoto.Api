using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VotoModelos;

    public class VotoMVCContext : DbContext
    {
        public VotoMVCContext (DbContextOptions<VotoMVCContext> options)
            : base(options)
        {
        }

        public DbSet<VotoModelos.Votante> Votante { get; set; } = default!;

public DbSet<VotoModelos.Voto> Voto { get; set; } = default!;

public DbSet<VotoModelos.Papeleta> Papeleta { get; set; } = default!;

public DbSet<VotoModelos.Auditoria> Auditoria { get; set; } = default!;
    }
