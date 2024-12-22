using GestaoClientes_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoClientes_backend.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Logradouro> Logradouros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Logradouro)
                .WithOne()
                .HasForeignKey(l => l.IdCliente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Logradouro>()
               .HasOne(l => l.Cliente)
               .WithMany(c => c.Logradouro)
               .HasForeignKey(l => l.IdCliente);
        }

    }
}