using Auth.Data.DTOs;
using FluentValidation;

namespace Auth.Validators
{
    public class RegistrationValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationValidator() 
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required!")
                .MinimumLength(3).WithMessage("First name must be at least 3 characters long.") // Shortest name could be Sam
                .MaximumLength(100).WithMessage("First name must be at most 100 characters long.");

            RuleFor(user=> user.LastName)
                .NotEmpty().WithMessage("Last name is required!")
                .MinimumLength(3).WithMessage("Last Name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Last name must be at most 100 characters long.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required!")
                .EmailAddress().WithMessage("Invalid Email");

            RuleFor(user => user.Password.Trim())
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Must(p => p.Any(char.IsUpper)).WithMessage("Need at least one uppercase letter.")
                .Must(p => p.Any(char.IsLower)).WithMessage("Need at least one lowercase letter.")
                .Must(p => p.Any(char.IsDigit)).WithMessage("Need at least one number.")
                .Must(p => p.Any(ch => !char.IsLetterOrDigit(ch))).WithMessage("Need at least one special character.")
                .Must((model, password) => !password.Contains(model.FirstName))
                .WithMessage("Password cannot contain your name.")
                .Must((model, password) => !password.Contains(model.LastName))
                .WithMessage("Password cannot contain your name.");

            RuleFor(user => user.ConfirmPassword.Trim())
                .NotEmpty().WithMessage("Please confirm your password.")
                .Must((model, confirmPassword) => confirmPassword == model.Password.Trim())
                .WithMessage("Passwords do not match.");
        }
    }
}
