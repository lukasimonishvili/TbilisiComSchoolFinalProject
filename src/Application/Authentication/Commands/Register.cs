using Application.Authentication.Validators;
using Domain.DTO.Authentication;
using Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Application.Authentication.Commands
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class Register : Controller
    {
        private readonly IRegisterService _registerService;
        private readonly ILogger<Register> _logger;

        public Register(IRegisterService registerService, ILogger<Register> logger)
        {
            _registerService = registerService;
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDTO user)
        {
            var validator = new RegisterValidator().Validate(user);
            if (!validator.IsValid)
            {
                var message = validator.Errors.Count > 1 ? "More then 1 validation error detected" : validator.Errors[0].ErrorMessage;
                return BadRequest(message);
            }
            var port = Request.Host.Port.HasValue ? $":{Request.Host.Port.Value}" : "";
            var emailActivateUrl = $"{Request.Scheme}://{Request.Host.Host}{port}/api/confirm-registration?email=";
            var result = await _registerService.RegisterUser(user, emailActivateUrl);

            if (result == "usernameConflict")
            {
                return Conflict($"user with username {user.Username} already exists");
            }


            if (result == "EmailConflict")
            {
                return Conflict($"user with email {user.Email} already exists");
            }

            if (result == "senderError")
            {
                return Conflict($"email sender is under maintenance. please try to register later");
            }

            return Ok(result);

        }

        [HttpGet("/api/confirm-registration")]
        public IActionResult ConfirmRegistration([FromQuery] string email)
        {

            var result = _registerService.ActivateUser(email);

            if (result == "notFound")
            {
                return NotFound($"user with email {email} not found in system");
            }

            if (result == "Verified")
            {
                return Conflict("Your account is already verified");
            }

            return Ok($"Your account is now activated");
        }
    }

}
