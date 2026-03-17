namespace Campaign.Infrastructure.EntityConfigurations;

public class DonationIntentDeadLetterConfiguration : IEntityTypeConfiguration<DonationIntentDeadLetter>
{
    public void Configure(EntityTypeBuilder<DonationIntentDeadLetter> builder)
    {
        builder.ToTable("donation_intent_dead_letters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder.Property(x => x.DonationIntentId)
            .HasColumnName("donation_intent_id")
            .IsRequired();

        builder.Property(x => x.MessageId)
            .HasColumnName("message_id")
            .HasMaxLength(100);

        builder.Property(x => x.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.FailedAt)
            .HasColumnName("failed_at")
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}
