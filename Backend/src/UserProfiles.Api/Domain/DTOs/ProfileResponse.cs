namespace UserProfiles.Api.Domain.DTOs
{
    public record ProfileResponse(
        string FirstName,
        string LastName,
        string Email,
        bool IsVerified
        );
    
}
