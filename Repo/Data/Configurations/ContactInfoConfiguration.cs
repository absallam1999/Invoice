using invoice.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class ContactInfoConfiguration : BaseEntityConfiguration<ContactInfo>
    {
        public override void Configure(EntityTypeBuilder<ContactInfo> builder)
        {
            base.Configure(builder);

            builder.ToTable("ContactInformation");

            builder.Property(c => c.Phone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Location)
                .HasMaxLength(250);

            builder.Property(c => c.Facebook)
                .HasMaxLength(250);

            builder.Property(c => c.WhatsApp)
                .HasMaxLength(250);

            builder.Property(c => c.Instagram)
                .HasMaxLength(250);

            builder.HasOne(c => c.Store)
                .WithMany(s => s.ContactInformations)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
