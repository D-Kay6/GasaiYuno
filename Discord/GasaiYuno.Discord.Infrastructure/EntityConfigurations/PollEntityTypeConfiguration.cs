using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class PollEntityTypeConfiguration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.ToTable("Polls");

            builder.Property<ulong>("ServerId")
                .IsRequired();

            builder.Property(x => x.Channel)
                .IsRequired();

            builder.Property(x => x.Message)
                .IsRequired();

            builder.Property(x => x.MultiSelect)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.Text)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Options)
                .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<List<PollOption>>(x));

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey("ServerId", nameof(Poll.Channel), nameof(Poll.Message));
        }
    }
}