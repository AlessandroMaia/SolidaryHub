namespace Campaign.Infrastructure.EntityConfigurations;

public class CampaignStatusHistoryConfiguration : IEntityTypeConfiguration<CampaignStatusHistory>
{
    public void Configure(EntityTypeBuilder<CampaignStatusHistory> builder)
    {
        builder.ToTable("campaign_status_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder.Property(x => x.CampaignId)
            .HasColumnName("campaign_id")
            .IsRequired();

        builder.Property(x => x.OldStatus)
            .HasColumnName("old_status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.NewStatus)
            .HasColumnName("new_status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasColumnName("reason")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.ChangedByUserId)
            .HasColumnName("changed_by_user_id")
            .IsRequired();

        builder.Property(x => x.ChangedAt)
            .HasColumnName("changed_at")
            .IsRequired();

        builder.Ignore(r => r.DomainEvents);
    }
}