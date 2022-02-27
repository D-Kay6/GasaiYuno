using GasaiYuno.Discord.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class PollEntityTypeConfiguration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.ToTable("Polls");

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property<ulong>("ServerId")
                .IsRequired();

            builder.Property(x => x.Channel)
                .IsRequired();

            builder.Property(x => x.Message)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.Text)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasMany(x => x.Options)
                .WithOne()
                .HasForeignKey("PollId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Selections)
                .WithOne()
                .HasForeignKey("PollId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(x => x.Id);
        }
    }

    public class PollOptionEntityTypeConfiguration : IEntityTypeConfiguration<PollOption>
    {
        public void Configure(EntityTypeBuilder<PollOption> builder)
        {
            builder.ToTable("PollOptions");

            builder.Property<ulong>("PollId")
                .IsRequired();

            builder.Property(x => x.Value)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasKey("PollId", nameof(PollOption.Value));
        }
    }

    public class PollSelectionEntityTypeConfiguration : IEntityTypeConfiguration<PollSelection>
    {
        public void Configure(EntityTypeBuilder<PollSelection> builder)
        {
            builder.ToTable("PollSelections");

            builder.Property<ulong>("PollId")
                .IsRequired();

            builder.Property(x => x.User)
                .IsRequired();

            builder.Property(x => x.SelectedOption)
                .IsRequired();

            builder.HasKey("PollId", nameof(PollSelection.User), nameof(PollSelection.SelectedOption));
        }
    }
}