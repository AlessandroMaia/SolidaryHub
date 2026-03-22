namespace Campaign.Infrastructure.EntityConfigurations;

public class CampaignConfiguration : IEntityTypeConfiguration<CampaignEntity>
{
    public void Configure(EntityTypeBuilder<CampaignEntity> builder)
    {
        builder.ToTable("campaigns");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder.Property(c => c.Title)
            .HasColumnName("title")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired();

        builder.Ignore(c => c.StartDate);
        builder.Ignore(c => c.EndDate);
        builder.Ignore(c => c.FinancialGoalAmount);

        builder.OwnsOne(c => c.Period, period =>
        {
            period.Property(p => p.StartDate)
                .HasColumnName("start_date")
                .IsRequired();

            period.Property(p => p.EndDate)
                .HasColumnName("end_date")
                .IsRequired();
        });

        builder.OwnsOne(c => c.FinancialGoal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("financial_goal_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("financial_goal_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.OwnsOne(c => c.Total, total =>
        {
            total.Property(t => t.TotalAmountRaised)
                .HasColumnName("total_amount_raised")
                .HasPrecision(18, 2)
                .IsRequired();

            total.Property(t => t.TotalDonationsCount)
                .HasColumnName("total_donations_count")
                .IsRequired();

            total.Property(t => t.LastDonationAt)
                .HasColumnName("last_donation_at");

            total.Property(t => t.UpdatedAt)
                .HasColumnName("total_updated_at")
                .IsRequired();
        });

        builder.HasMany(c => c.StatusHistory)
            .WithOne()
            .HasForeignKey(sh => sh.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.DonationLedger)
            .WithOne()
            .HasForeignKey(dl => dl.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.DomainEvents);
    }
}
