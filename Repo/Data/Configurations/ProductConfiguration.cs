using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.ToTable("Products");

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.MainImage)
                   .HasMaxLength(500);

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.Quantity)
                   .HasDefaultValue(0);

            builder.Property(p => p.InProductList)
                   .HasDefaultValue(true);

            builder.Property(p => p.InPOS)
                   .HasDefaultValue(true);

            builder.Property(p => p.InStore)
                   .HasDefaultValue(true);



            builder.HasOne(p => p.User)
                   .WithMany(u => u.Products)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            //builder.HasOne(p => p.Store)
            //       .WithMany(s => s.Products)
            //       .HasForeignKey(p => p.StoreId)
            //       .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.InvoiceItems)
                   .WithOne(ii => ii.Product)
                   .HasForeignKey(ii => ii.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}