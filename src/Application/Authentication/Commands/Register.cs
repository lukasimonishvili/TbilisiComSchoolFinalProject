using Application.Authentication.Validators;
using Domain.DTO.Authentication;
using Domain.Interface;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
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

            try
            {
                var port = Request.Host.Port.HasValue ? $":{Request.Host.Port.Value}" : "";
                var emailActivateUrl = $"{Request.Scheme}://{Request.Host.Host}{port}/api/confirm-registration?email=";
                var result = await _registerService.RegisterUser(user, emailActivateUrl);
                return Ok(result);

            }
            catch (SmtpException)
            {

                return Conflict($"email sender is under maintenance. please try to register later");
            }
            catch (UserExistsException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("/api/confirm-registration")]
        public IActionResult ConfirmRegistration([FromQuery] string email)
        {
            try
            {
                _registerService.ActivateUser(email);
                return Ok($"Your account is now activated");

            }
            catch (DataNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserIsAlreadyVerifiedException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }

}
