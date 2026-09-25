using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdvSistema.Domain.Entities;

namespace PdvSistema.Infrastructure.Data.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.HasKey(v => v.Id);

        builder.HasOne(v => v.Cliente)
            .WithMany()
            .HasForeignKey("ClienteId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.Itens)
            .WithOne(i => i.Venda)
            .HasForeignKey("VendaId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}