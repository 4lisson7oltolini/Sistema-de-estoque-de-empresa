using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdvSistema.Domain.Entities;

namespace PdvSistema.Infrastructure.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey("CategoriaId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}