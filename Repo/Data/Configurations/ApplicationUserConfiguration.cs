using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(u => u.DeletedAt);

            builder.HasQueryFilter(u => !u.IsDeleted);

            builder.HasOne(u => u.Store)
                   .WithOne(s => s.User)
                   .HasForeignKey<Store>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Invoices)
                   .WithOne(i => i.User)
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Clients)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Products)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Notifications)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
