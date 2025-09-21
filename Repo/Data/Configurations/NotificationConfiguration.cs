using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class NotificationConfiguration : BaseEntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.ToTable("Notifications");

            builder.Property(n => n.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(n => n.Type)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
