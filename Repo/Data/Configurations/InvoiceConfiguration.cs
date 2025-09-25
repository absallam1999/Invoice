//using invoice.Core.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace invoice.Repo.Data.Configurations
//{
//    public class InvoiceConfiguration : BaseEntityConfiguration<Invoice>
//    {
//        public override void Configure(EntityTypeBuilder<Invoice> builder)
//        {
//            base.Configure(builder);

//            builder.ToTable("Invoices");

//            builder.Property(i => i.Code)
//                   .IsRequired()
//                   .HasMaxLength(50);

//            builder.HasIndex(i => i.Code)
//                   .IsUnique();

//            //builder.Property(i => i.Currency)
//            //       .IsRequired()
//            //       .HasMaxLength(10);

//            builder.Property(i => i.Value)
//                   .IsRequired()
//                   .HasColumnType("decimal(18,2)");

//            builder.Property(i => i.DiscountValue)
//                   .HasColumnType("decimal(18,2)");

//            builder.Property(i => i.FinalValue)
//                   .IsRequired()
//                   .HasColumnType("decimal(18,2)");

//            builder.Property(i => i.Tax)
//                   .IsRequired();

//            builder.Property(i => i.TermsConditions)
//                   .HasMaxLength(2000);

//            builder.Property(i => i.InvoiceStatus)
//                   .IsRequired();

//            builder.Property(i => i.InvoiceType)
//                   .IsRequired();

//            builder.HasOne(i => i.User)
//                   .WithMany(u => u.Invoices)
//                   .HasForeignKey(i => i.UserId)
//                   .OnDelete(DeleteBehavior.Restrict);



//            builder.HasOne(i => i.Client)
//                   .WithMany(c => c.Invoices)
//                   .HasForeignKey(i => i.ClientId)
//                   .OnDelete(DeleteBehavior.NoAction);

//            builder.HasOne(i => i.Language)
//                   .WithMany(l => l.Invoices)
//                   .HasForeignKey(i => i.LanguageId)
//                   .OnDelete(DeleteBehavior.Restrict);

//            builder.HasOne(i => i.PayInvoice)
//                   .WithOne(pi => pi.Invoice)
//                   .HasForeignKey<PayInvoice>(pi => pi.InvoiceId)
//                   .OnDelete(DeleteBehavior.NoAction);

//            builder.HasOne(i => i.Order)
//                   .WithOne(o => o.Invoice)
//                   .HasForeignKey<Invoice>(i => i.Id)
//                   .OnDelete(DeleteBehavior.Restrict);



//                builder.HasOne(i => i.PaymentLink)
//           .WithOne(pl => pl.Invoice)
//           .HasForeignKey<PaymentLink>(pl => pl.InvoiceId)
//           .OnDelete(DeleteBehavior.NoAction);


//            builder.HasMany(i => i.Payments)
//                   .WithOne(p => p.Invoice)
//                   .HasForeignKey(p => p.InvoiceId)
//                   .OnDelete(DeleteBehavior.Cascade);

//            builder.HasMany(i => i.InvoiceItems)
//                   .WithOne(ii => ii.Invoice)
//                   .HasForeignKey(ii => ii.InvoiceId)
//                   .OnDelete(DeleteBehavior.Cascade);
//        }
//    }
//}

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

            //builder.Property(i => i.TaxNumber)
            //    .HasMaxLength(100);

            builder.Property(i => i.Value).HasColumnType("decimal(18,2)");
            //builder.Property(i => i.TotalPaid).HasColumnType("decimal(18,2)");
            //builder.Property(i => i.RemainingAmount).HasColumnType("decimal(18,2)");
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

            

            builder.HasOne(i => i.PaymentLink)
            .WithMany(pl => pl.Invoices)
            .HasForeignKey(i => i.PaymentLinkId)
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
