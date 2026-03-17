namespace Campaign.Infrastructure.EntityConfigurations;

public class DonationIntentConfiguration : IEntityTypeConfiguration<DonationIntent>
{
    public void Configure(EntityTypeBuilder<DonationIntent> builder)
    {
        builder.ToTable("donation_intents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder.Property(x => x.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(x => x.DonorUserId)
            .HasColumnName("donor_user_id")
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasColumnName("currency")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Source)
            .HasColumnName("source")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(100);

        builder.Property(x => x.MessageId)
            .HasColumnName("message_id")
            .HasMaxLength(100);

        builder.Property(x => x.RequestedAt)
            .HasColumnName("requested_at")
            .IsRequired();

        builder.Property(x => x.ValidatedAt)
            .HasColumnName("validated_at");

        builder.Property(x => x.RejectedAt)
            .HasColumnName("rejected_at");

        builder.Property(x => x.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(x => x.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasMaxLength(300);

        builder.HasMany(x => x.ProcessingLogs)
            .WithOne()
            .HasForeignKey(x => x.DonationIntentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.DeadLetters)
            .WithOne()
            .HasForeignKey(x => x.DonationIntentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.MessageId)
            .IsUnique(false);

        builder.Ignore(x => x.DomainEvents);
    }
}
