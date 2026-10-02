using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdvSistema.Domain.Entities;

namespace PdvSistema.Infrastructure.Data;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.HasKey(categoria => categoria.Id);
        builder.Property(categoria => categoria.Nome)
            .IsRequired()
            .HasMaxLength(100);
    }
}