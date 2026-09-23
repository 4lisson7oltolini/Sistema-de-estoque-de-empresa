using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;

namespace PdvSistema.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Venda> Vendas => Set<Venda>();
        public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.Property(p => p.Nome).IsRequired().HasMaxLength(150);
                entity.Property(p => p.PrecoCusto).HasColumnType("decimal(10,2)");
                entity.Property(p => p.PrecoVenda).HasColumnType("decimal(10,2)");
                entity.HasOne(p => p.Categoria)
                      .WithMany()
                      .HasForeignKey(p => p.CategoriaId);
            });

            modelBuilder.Entity<Venda>(entity =>
            {
                entity.Property(v => v.ValorTotal).HasColumnType("decimal(10,2)");
                entity.HasMany(v => v.Itens)
                      .WithOne()
                      .HasForeignKey(i => i.VendaId);
            });

            modelBuilder.Entity<ItemVenda>(entity =>
            {
                entity.Property(i => i.PrecoUnitario).HasColumnType("decimal(10,2)");
            });
        }
    }
}