using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdvSistema.Domain.Entities;

namespace PdvSistema.Infrastructure.Data;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Data)
            .IsRequired();

        builder.Property(v => v.UsuarioId)
            .IsRequired();

        builder.Property(v => v.FormaPagamento)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.Status)
            .HasMaxLength(20)
            .IsRequired();

        // Venda -> Cliente
        builder.HasOne(v => v.Cliente)
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Venda -> ItensVenda
        builder.HasMany(v => v.Itens)
            .WithOne(i => i.Venda)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}