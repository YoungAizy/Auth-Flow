using System.Linq;
using Auth.Data.DTOs;
using Auth.Validators;
using FluentValidation.Results;
using Xunit;

namespace Auth.Tests.Validators
{
    public class ValidatorsTests
    {
        [Fact]
        public void Registration_ValidModel_PassesValidation()
        {
            var validator = new RegistrationValidator();
            var dto = new RegistrationRequest(
                "Jon", "Mac Doe", 
                "john.doe@example.com", 
                "P@ssw0rd", "P@ssw0rd");

            var result = validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Registration_HypenatedNames_PassessValidation() 
        {
            RegistrationValidator validator = new RegistrationValidator();
            var dto = new RegistrationRequest(
                "Jon Matthew", "Mc-Doe",
                "jon.doe@example.com",
                "P@ssword100", "P@ssword100");

            ValidationResult result = validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Registration_PasswordContainsFirstName_FailsValidation()
        {
            var validator = new RegistrationValidator();
            var dto = new RegistrationRequest(
                "John", "Doe", "john@example.com", "John123!", "John123!");

            var result = validator.Validate(dto);
            //var error = result.Errors.FirstOrDefault();

            Assert.False(result.IsValid);

            //bool hasError = result.Errors.Any(e => 
            //    e.PropertyName == "Password" &&
            //    e.ErrorMessage.Contains("cannot contain"));

            //Assert.True(hasError);

            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("cannot contain"));
        }

        [Fact]
        public void Registration_ConfirmPasswordMismatch_FailsValidation()
        {
            var validator = new RegistrationValidator();
            var dto = new RegistrationRequest(
                "John", "Doe", "john@example.com", "P@ssw0rd", "Different");

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Passwords do not match.");
        }

        [Fact]
        public void Registration_ShortFirstName_FailsValidation()
        {
            var validator = new RegistrationValidator();
            var dto = new RegistrationRequest(
                "Jo", "Doe", "john@example.com", "P@ssw0rd", "P@ssw0rd");

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
        }

        [Fact]
        public void Registration_InvalidEmail_FailsValidation()
        {
            var validator = new RegistrationValidator();
            var dto = new RegistrationRequest(
                "John", "Doe", "not-an-email", "P@ssw0rd", "P@ssw0rd");

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void Login_ValidModel_PassesValidation()
        {
            var validator = new LoginValidator();
            var dto = new LoginRequest("jane.doe@example.com", "P@ssw0rd");

            var result = validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Login_InvalidEmail_FailsValidation()
        {
            var validator = new LoginValidator();
            var dto = new LoginRequest("not-an-email", "P@ssw0rd");

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void Login_PasswordMissingUppercase_FailsValidation()
        {
            var validator = new LoginValidator();
            var dto = new LoginRequest("jane@example.com", "p@ssw0rd1");

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
        }

        [Fact]
        public void Login_ShortPassword_FailsValidation()
        {
            var validator = new LoginValidator();
            var dto = new LoginRequest("jane@example.com", "P1!");

            var result = validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
        }
    }
}
