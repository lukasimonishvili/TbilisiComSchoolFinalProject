using Application.Authentication.Validators;
using Domain.DTO.Authentication;
using Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Application.Authentication.Commands
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class Register : Controller
    {
        private readonly IRegisterService _registerService;

        public Register(IRegisterService registerService)
        {
            _registerService = registerService;
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
                return Conflict($"User user with email - {user.Email} already exists");
            }

            DBuser = _registerService.GetUserByUsername(user.Username);
            if (DBuser != null)
            {
                return Conflict($"User with username {user.Username} already exists");
            }


            try
            {
                var port = Request.Host.Port.HasValue ? $":{Request.Host.Port.Value}" : "";
                var emailActivateUrl = $"{Request.Scheme}://{Request.Host.Host}{port}/api/confirm-registration?email={user.Email}";
                await _registerService.SendRegisterConfirmationEmail(user.Email, user.Username, emailActivateUrl);
                _registerService.RegisterUser(user);
                return Ok($"check your email: {user.Email} to confirm registration");
            }
            catch
            {
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
