using FluentValidation;
using UserProfiles.Api.Domain.DTOs;

namespace UserProfiles.Api.Application.Validators
{
    public class CreateProfileValidator : AbstractValidator<CreateProfileRequest>
    {
        public CreateProfileValidator() 
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required!")
                .Length(3, 50).WithMessage("First name must be between 3 - 50 characters long."); // Shortest name could be Sam

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required!")
                .Length(3, 50).WithMessage("Last Name must be between 3 - 50 characters long.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required!")
                .EmailAddress().WithMessage("Invalid Email");
        }
    }
}
