using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class InvoiceConfiguration : BaseEntityConfiguration<Invoice>
    {
        public override void Configure(EntityTypeBuilder<Invoice> builder)
        {
            base.Configure(builder);

            builder.ToTable("Invoices");

            builder.Property(i => i.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Value).HasColumnType("decimal(18,2)");
            builder.Property(i => i.DiscountValue).HasColumnType("decimal(18,2)");
            builder.Property(i => i.FinalValue).HasColumnType("decimal(18,2)");

            builder.Property(i => i.TermsConditions)
                .HasMaxLength(2000);

            builder.HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Client)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(i => i.Language)
                .WithMany(l => l.Invoices)
                .HasForeignKey(i => i.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Payments)
                .WithOne(p => p.Invoice)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(i => i.InvoiceItems)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.PayInvoice)
               .WithOne(pi => pi.Invoice)
               .HasForeignKey<PayInvoice>(pi => pi.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
