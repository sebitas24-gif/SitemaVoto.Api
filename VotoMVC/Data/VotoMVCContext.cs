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

   

public DbSet<VotoModelos.Papeleta> Papeleta { get; set; } = default!;



public DbSet<VotoModelos.ProcesoElectoral> ProcesoElectoral { get; set; } = default!;






























public DbSet<VotoModelos.Candidato> Candidato { get; set; } = default!;


}
