using Claims.Domain.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Claims.Infrastructure.Persistence.Configurations;

public sealed class ClaimInformationRequestConfiguration :
    IEntityTypeConfiguration<ClaimInformationRequest>
{
    public void Configure(
        EntityTypeBuilder<ClaimInformationRequest> builder)
    {
        builder.ToTable("claim_information_requests");

        builder.HasKey(request => request.Id);

        builder.Property(request => request.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(request => request.ClaimId)
            .HasColumnName("claim_id")
            .IsRequired();

        builder.Property(request => request.Question)
            .HasColumnName("question")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(request => request.Response)
            .HasColumnName("response")
            .HasMaxLength(4000);

        builder.Property(request => request.RequestedByOfficerId)
            .HasColumnName("requested_by_officer_id")
            .IsRequired();

        builder.Property(request => request.RequestedAt)
            .HasColumnName("requested_at")
            .IsRequired();

        builder.Property(request => request.RespondedAt)
            .HasColumnName("responded_at");

        builder.Property(request => request.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Ignore(request => request.DomainEvents);

        builder.HasIndex(request => request.ClaimId);

        builder.HasIndex(request => new
        {
            request.ClaimId,
            request.Status
        });
    }
}