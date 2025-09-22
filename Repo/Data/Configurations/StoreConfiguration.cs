using invoice.Core.Entities;
using invoice.Models.Entities.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class StoreConfiguration : BaseEntityConfiguration<Store>
    {
        public override void Configure(EntityTypeBuilder<Store> builder)
        {
            base.Configure(builder);

            builder.ToTable("Stores");

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(s => s.Description)
                   .HasMaxLength(1000);

            //builder.Property(s => s.Tax)
            //       .HasDefaultValue(false);

            builder.Property(s => s.IsActivated)
                   .HasDefaultValue(true);

            //builder.Property(s => s.PaymentMethod)
            //       .HasConversion<int>();

            builder.HasOne(s => s.User)
                   .WithOne(u => u.Store)
                   .HasForeignKey<Store>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(s => s.Language)
            //       .WithMany(l => l.Stores)
            //       .HasForeignKey(s => s.LanguageId)
            //       .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(s => s.Products)
            //       .WithOne(p => p.Store)
            //       .HasForeignKey(p => p.StoreId)
            //       .OnDelete(DeleteBehavior.SetNull);

            //builder.HasMany(s => s.Invoices)
            //       .WithOne(i => i.Store)
            //       .HasForeignKey(i => i.StoreId)
            //       .OnDelete(DeleteBehavior.SetNull);

            //builder.HasMany(s => s.ContactInformations)
            //       .WithOne(c => c.Store)
            //       .HasForeignKey(c => c.StoreId)
            //       .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Pages)
                   .WithOne(p => p.Store)
                   .HasForeignKey(p => p.StoreId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.OwnsOne(s => s.Shipping, shipping =>
            {
                shipping.Property(sh => sh.FromStore)
                        .HasDefaultValue(true);


            });

            builder.OwnsOne(s => s.StoreSettings, settings =>
            {
                //settings.Property(ss => ss.Url)
                //        .IsRequired()
                //        .HasMaxLength(200);

                settings.Property(ss => ss.Logo)
                        .HasMaxLength(500);

                settings.Property(ss => ss.CoverImage)
                        .HasMaxLength(500);

                settings.Property(ss => ss.Color)
                        .HasMaxLength(20)
                        .HasDefaultValue("#FFFFFF");

                settings.Property(ss => ss.Currency)
                        .HasMaxLength(10)
                        .HasDefaultValue("USD");

                settings.OwnsOne(ss => ss.purchaseOptions, options =>
                {
                    options.Property(po => po.Name).HasDefaultValue(true);
                    options.Property(po => po.Email).HasDefaultValue(false);
                    options.Property(po => po.phone).HasDefaultValue(false);

                    options.Property(po => po.TermsAndConditions)
                           .HasMaxLength(2000);
                });
            });
        }
    }
}