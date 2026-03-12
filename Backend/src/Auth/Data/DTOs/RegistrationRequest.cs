namespace Auth.Data.DTOs
{
    public record RegistrationRequest(
        string FirstName,
        string LastName, 
        string Email, 
        string Password,
        string ConfirmPassword
        );
}
