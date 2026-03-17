namespace Identity.Infrastructure.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.Ignore(r => r.DomainEvents);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .UseIdentityColumn();

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
    }
}
