namespace Campaign.Infrastructure.EntityConfigurations;

public class DonationLedgerEntryConfiguration : IEntityTypeConfiguration<DonationLedgerEntry>
{
    public void Configure(EntityTypeBuilder<DonationLedgerEntry> builder)
    {
        builder.ToTable("donation_ledger_entries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder.Property(x => x.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(x => x.DonationIntentId)
            .HasColumnName("donation_intent_id")
            .IsRequired();

        builder.Property(x => x.DonorUserId)
            .HasColumnName("donor_user_id")
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Source)
            .HasColumnName("source")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(100);

        builder.Property(x => x.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}
