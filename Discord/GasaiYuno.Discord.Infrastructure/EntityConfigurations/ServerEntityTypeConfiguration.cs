using GasaiYuno.Discord.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class ServerEntityTypeConfiguration : IEntityTypeConfiguration<Server>
    {
        public void Configure(EntityTypeBuilder<Server> builder)
        {
            builder.ToTable("Servers");

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.WarningDisabled)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.Prefix)
                .HasDefaultValue("!")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Language)
                .HasDefaultValue(Languages.English)
                .IsRequired();

            builder.HasKey(x => x.Id);
        }
    }
}