namespace Claims.Domain.Claims;

public enum ClaimStatus
{
    Submitted = 1,
    UnderReview = 2,
    AdditionalInformationRequired = 3,
    Approved = 4,
    Rejected = 5,
    Settled = 6
}