using Domain.DTO.Authentication;
using Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Application.Authentication.Commands
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class Login : Controller
    {
        private readonly ILoginService _loginService;

        public Login(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("")]
        public IActionResult Authenticate([FromBody] LoginDTO loginDto)
        {
            var result = _loginService.Authenticate(loginDto);

            if (result == null)
            {
                return Unauthorized("Wron username or password");
            }

            if (result == "Unverified")
            {
                return Conflict("Please confirm email to finish registration");
            }

            return Ok(result);
        }

        [HttpPost("/api/refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenDTO data)
        {
            var refreshToken = _loginService.RefreshToken(data.Token);

            if (refreshToken == null)
            {
                return Unauthorized("Invalid Token Detected");
            }

            if (refreshToken == "Valid")
            {
                return Conflict("Token is not expired.");
            }

            return Ok(refreshToken);
        }
    }
}
