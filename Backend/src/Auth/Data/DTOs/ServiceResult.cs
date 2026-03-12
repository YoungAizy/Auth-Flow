namespace Auth.Data.DTOs
{
    public record ServiceResult(
        bool IsSuccess, 
        string? Failure = null, 
        string? Error = null);
}
