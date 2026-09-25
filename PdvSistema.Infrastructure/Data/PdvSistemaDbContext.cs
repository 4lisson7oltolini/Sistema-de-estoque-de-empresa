using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;

namespace PdvSistema.Infrastructure.Data;

public class PdvSistemaDbContext : DbContext
{
    public PdvSistemaDbContext(DbContextOptions<PdvSistemaDbContext> options)
        : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PdvSistemaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}