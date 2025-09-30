using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
    {
        public void Configure(EntityTypeBuilder<Commission> builder)
        {
            builder.ToTable("Commissions");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Value)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(c => c.InvoiceId)
                   .IsRequired();

            builder.Property(c => c.SellerId)
                   .IsRequired();

            builder.Property(c => c.Settled)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.HasOne(c => c.Invoice)
                   .WithOne(i => i.Commission)
                   .HasForeignKey<Commission>(c => c.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
