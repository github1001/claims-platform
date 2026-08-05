using Claims.Domain.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Claims.Infrastructure.Persistence.Configurations;

public sealed class ClaimConfiguration :
    IEntityTypeConfiguration<Claim>
{
    public void Configure(
        EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("claims");

        builder.HasKey(claim => claim.Id);

        builder.Property(claim => claim.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(claim => claim.ClaimNumber)
            .HasColumnName("claim_number")
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(claim => claim.ClaimNumber)
            .IsUnique();

        builder.Property(claim => claim.ClaimType)
            .HasColumnName("claim_type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(claim => claim.ClaimantName)
            .HasColumnName("claimant_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(claim => claim.ClaimantEmail)
            .HasColumnName("claimant_email")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(claim => claim.IncidentDate)
            .HasColumnName("incident_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(claim => claim.IncidentDescription)
            .HasColumnName("incident_description")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(claim => claim.EstimatedLoss)
            .HasColumnName("estimated_loss")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(claim => claim.ApprovedSettlementAmount)
            .HasColumnName("approved_settlement_amount")
            .HasPrecision(18, 2);

        builder.Property(claim => claim.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(claim => claim.AssignedOfficerId)
            .HasColumnName("assigned_officer_id");

        builder.Property(claim => claim.DecisionReason)
            .HasColumnName("decision_reason")
            .HasMaxLength(2000);

        builder.Property(claim => claim.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(claim => claim.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(claim => claim.ClosedAt)
            .HasColumnName("closed_at");

        builder.Property(claim => claim.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.HasMany(claim => claim.InformationRequests)
            .WithOne()
            .HasForeignKey(request => request.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Claim.InformationRequests))!
            .SetField("_informationRequests");

        builder.Navigation(claim => claim.InformationRequests)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(claim => claim.DomainEvents);

        builder.Navigation(claim => claim.InformationRequests)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}