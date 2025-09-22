using invoice.Core.Entities;
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

            builder.Property(pl => pl.Value)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(pl => pl.Currency)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(pl => pl.Purpose)
                   .HasMaxLength(250);

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

            builder.Property(pl => pl.CreatedBy)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(pl => pl.IsActive)
                   .HasDefaultValue(true);

            //builder.HasOne(pl => pl.Invoice)
            //       .WithOne(i => i.PaymentLink)
            //       .HasForeignKey<PaymentLink>(pl => pl.InvoiceId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
