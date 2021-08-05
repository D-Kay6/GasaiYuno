using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.Property<ulong>("ServerId")
                .IsRequired();

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.Message)
                .HasMaxLength(2000)
                .HasDefaultValue("Welcome to the party {0}. Hope you will have a good time with us.")
                .IsRequired();

            builder.Property(x => x.Image)
                .IsRequired(false);

            builder.Property(x => x.Channel)
                .IsRequired(false);

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey("ServerId", nameof(Notification.Type));
        }
    }
}