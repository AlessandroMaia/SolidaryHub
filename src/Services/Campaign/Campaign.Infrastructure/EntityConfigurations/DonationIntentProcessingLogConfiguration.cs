namespace Campaign.Infrastructure.EntityConfigurations;

public class DonationIntentProcessingLogConfiguration : IEntityTypeConfiguration<DonationIntentProcessingLog>
{
    public void Configure(EntityTypeBuilder<DonationIntentProcessingLog> builder)
    {
        builder.ToTable("donation_intent_processing_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder.Property(x => x.DonationIntentId)
            .HasColumnName("donation_intent_id")
            .IsRequired();

        builder.Property(x => x.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(x => x.WorkerName)
            .HasColumnName("worker_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AttemptCount)
            .HasColumnName("attempt_count")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.LastAttemptAt)
            .HasColumnName("last_attempt_at")
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("text");

        builder.Ignore(x => x.DomainEvents);
    }
}
