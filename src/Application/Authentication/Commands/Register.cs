using Application.Authentication.Validators;
using Domain.DTO.Authentication;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Application.Authentication.Commands
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class Register : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<Register> _logger;

        public Register(AuthService authService, ILogger<Register> logger)
        {
            _authService = authService;
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


            var DBuser = _authService.GetUserByEmail(user.Email);
            if (DBuser != null)
            {
                return Conflict($"User user with email - {user.Email} already exists");
            }

            DBuser = _authService.GetUserByUsername(user.Username);
            if (DBuser != null)
            {
                return Conflict($"User with username {user.Username} already exists");
            }


            try
            {
                var port = Request.Host.Port.HasValue ? $":{Request.Host.Port.Value}" : "";
                var emailActivateUrl = $"{Request.Scheme}://{Request.Host.Host}{port}/api/confirm-registration?email={user.Email}";
                await _authService.SendRegisterConfirmationEmail(user.Email, user.Username, emailActivateUrl);
                _authService.RegisterUser(user);
                return Ok($"check your email: {user.Email} to confirm registration");
            }
            catch (Exception)
            {
                _logger.LogInformation("karateeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
                return Conflict("Email sender system is under maintenance. Thank you for your patience please try to register later");
            }


        }
    }
}
