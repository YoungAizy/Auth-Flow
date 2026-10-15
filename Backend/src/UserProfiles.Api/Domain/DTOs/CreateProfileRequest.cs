namespace UserProfiles.Api.Domain.DTOs
{
    public record CreateProfileRequest
    (
        string Email,
        string FirstName,
        string LastName,
        string Sub
    );
}
