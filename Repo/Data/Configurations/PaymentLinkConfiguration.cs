using invoice.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class PaymentLinkConfiguration : BaseEntityConfiguration<PaymentLink>
    {
        public override void Configure(EntityTypeBuilder<PaymentLink> builder)
        {
            base.Configure(builder);

            builder.ToTable("PaymentLinks");

            builder.Property(pl => pl.Link)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(pl => pl.Value)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(pl => pl.PaymentsNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(pl => pl.Description)
                   .HasMaxLength(1000);

            builder.Property(pl => pl.Message)
                   .HasMaxLength(1000);

            builder.Property(pl => pl.Image)
                   .HasMaxLength(500);

            builder.Property(pl => pl.Terms)
                   .HasMaxLength(2000);

            builder.HasOne(pl => pl.Invoice)
                   .WithMany(i => i.PaymentLinks)
                   .HasForeignKey(pl => pl.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pl => pl.Payments)
                   .WithOne(p => p.PaymentLink)
                   .HasForeignKey(p => p.PaymentLinkId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
