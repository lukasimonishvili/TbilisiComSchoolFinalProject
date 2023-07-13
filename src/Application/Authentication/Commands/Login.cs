using Domain.DTO.Authentication;
using Domain.Interface;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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
            try
            {
                var result = _loginService.Authenticate(loginDto);
                return Ok(result);

            }
            catch (WrongCredentialsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UserUnverifiedException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("/api/refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenDTO data)
        {
            try
            {
                var refreshToken = _loginService.RefreshToken(data.Token);
                return Ok(refreshToken);
            }
            catch (ArgumentException)
            {
                return Unauthorized("Invalid Token Detected");
            }
            catch (TokenNotExpiredException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("/api/GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                var result = _loginService.GetUserById(id, authorizationHeader);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
