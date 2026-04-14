using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PocketLedger.Infrastructure.Persistence.Models;

namespace PocketLedger.Infrastructure.Persistence.Configurations;

public sealed class SourceEventConfiguration : IEntityTypeConfiguration<SourceEventRecord>
{
    public void Configure(EntityTypeBuilder<SourceEventRecord> builder)
    {
        builder.ToTable("source_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.SourceType)
            .IsRequired();

        builder.Property(x => x.RawPayload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.Property(x => x.OccurredAt)
            .IsRequired(false);

        builder.Property(x => x.ExternalId)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.HasIndex(x => x.SourceType);
        builder.HasIndex(x => x.ReceivedAt);
        builder.HasIndex(x => x.ExternalId);
    }
}