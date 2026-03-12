using Auth.Data.DTOs;
using Auth.Services;
using Auth.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegistrationController(
        RegistrationService registrationService) : ControllerBase
    {

        // POST api/<AuthController>
        [HttpPost]
        public async Task<IActionResult> Register(
            [FromBody] RegistrationRequest userForm,
            CancellationToken ct)
        {
            RegistrationValidator validator = new RegistrationValidator();
            ValidationResult validationResult = validator.Validate(userForm);
            if (!validationResult.IsValid) 
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await registrationService.SaveProfile(userForm, ct);

            if (!result.IsSuccess)
            {
                if(result.Failure == "Email is already in use")
                {
                    return Conflict(new { message = result.Failure, error = result.Error });
                }

                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    new { message = result.Failure, error = result.Error });
            }
            

            return Ok();
        }
    }
}
