using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class CustomCommandEntityTypeConfiguration : IEntityTypeConfiguration<CustomCommand>
    {
        public void Configure(EntityTypeBuilder<CustomCommand> builder)
        {
            builder.ToTable("CustomCommands");

            builder.Property<ulong>("ServerId")
                .IsRequired();

            builder.Property(x => x.Command)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Response)
                .HasMaxLength(4000)
                .IsRequired();

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey("ServerId", nameof(CustomCommand.Command));
        }
    }
}