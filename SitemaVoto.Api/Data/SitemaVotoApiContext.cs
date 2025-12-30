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

        public DbSet<VotoModelos.Auditoria> Auditoria { get; set; } = default!;
    }
