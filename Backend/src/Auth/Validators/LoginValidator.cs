using Auth.Data.DTOs;
using FluentValidation;

namespace Auth.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(form => form.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(form => form.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Length(6,30).WithMessage("Password must be at least 6 characters long.")
                .Must(p => p.Any(char.IsUpper)).WithMessage("Need at least one uppercase letter.")
                .Must(p => p.Any(char.IsLower)).WithMessage("Need at least one lowercase letter.")
                .Must(p => p.Any(char.IsDigit)).WithMessage("Need at least one number.")
                .Must(p => p.Any(ch => !char.IsLetterOrDigit(ch))).WithMessage("Need at least one special character.");
        }
    }
}
