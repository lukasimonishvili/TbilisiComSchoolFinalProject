using Application.Authentication.Validators;
using Domain.DTO.Authentication;
using Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Mail;

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

            var DBuser = _registerService.GetUserByEmail(user.Email);
            if (DBuser != null)
            {
                _logger.LogWarning($"Attempted to register with already existed email - {user.Email}");
                return Conflict($"User user with email - {user.Email} already exists");
            }

            DBuser = _registerService.GetUserByUsername(user.Username);
            if (DBuser != null)
            {
                _logger.LogWarning($"Attempted to register with already existed username - {user.Username}");
                return Conflict($"User with username {user.Username} already exists");
            }


            try
            {
                var port = Request.Host.Port.HasValue ? $":{Request.Host.Port.Value}" : "";
                var emailActivateUrl = $"{Request.Scheme}://{Request.Host.Host}{port}/api/confirm-registration?email={user.Email}";
                await _registerService.SendRegisterConfirmationEmail(user.Email, user.Username, emailActivateUrl);
                _logger.LogInformation($"Confirmation email sent to address {user.Email}");
                _registerService.RegisterUser(user);
                return Ok($"check your email: {user.Email} to confirm registration");
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return Conflict("Email sender system is under maintenance. Thank you for your patience please try to register later");
            }
        }

        [HttpGet("/api/confirm-registration")]
        public IActionResult ConfirmRegistration([FromQuery] string email)
        {
            var user = _registerService.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound($"user with email address - {email} not found in system");
            }

            if (user.Verified)
            {
                return Conflict($"user with email address - {email} is already activated");
            }

            _registerService.ActivateUser(user);
            return Ok($"Your account is now activated");
        }
    }

}
