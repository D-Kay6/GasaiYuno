using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class BanEntityTypeConfiguration : IEntityTypeConfiguration<Ban>
    {
        public void Configure(EntityTypeBuilder<Ban> builder)
        {
            builder.ToTable("Bans");

            builder.Property<ulong>("ServerId")
                .IsRequired();

            builder.Property(x => x.User)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey("ServerId", nameof(Ban.User));
        }
    }
}