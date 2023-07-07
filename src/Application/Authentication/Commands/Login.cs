using Domain.DTO.Authentication;
using Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Authentication.Commands
{
    [Authorize]
    [ApiController]
    [Route("/api/[Controller]")]
    public class Login : Controller
    {
        private readonly ILoginService _loginService;

        public Login(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [HttpGet("/api/GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var result = _loginService.GetUserById(id, authorizationHeader);

            if (result == null)
            {
                return Unauthorized("You dont have permission to retrieve this data");

            }

            return Ok(result);
        }
    }
}
