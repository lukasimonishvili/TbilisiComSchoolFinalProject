
using BCryptNet = BCrypt.Net.BCrypt;
using Domain.DTO.Authentication;
using Domain.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Domain.Entities;
using Domain.Helpers;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mapster;
using Infrastructure.Exceptions;

namespace Infrastructure.Services
{
    public class LoginService : ILoginService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ILoginService> _logger;
        public LoginService(AppSettings appSettings, IUserRepository userRepository, ILogger<ILoginService> logger)
        {
            _appSettings = appSettings;
            _userRepository = userRepository;
            _logger = logger;
        }


        public string Authenticate(LoginDTO login)
        {
            var user = _userRepository.GetUserByUsername(login.Username);

            if (user == null)
            {
                _logger.LogInformation("Attempted to authenticate with incorrect username");
                throw new WrongCredentialsException();
            }

            if (!BCryptNet.Verify(login.Password, user.Password))
            {
                _logger.LogWarning($"attemt to authenticate with username {login.Username} failed because of wrong password");
                throw new WrongCredentialsException();
            }

            if (!user.Verified)
            {
                _logger.LogInformation($"Unverified user with id {user.Id} tried to authenticate");
                throw new UserUnverifiedException();
            }

            _logger.LogInformation($"user with username {login.Username} successfully authenticated");
            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("Email", user.Email),
                    new Claim("Role", user.Role),
                    new Claim("IsBlocked", user.IsBlocked.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public string RefreshToken(string tokenString)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");

            if (DateTime.UtcNow > token.ValidTo)
            {
                throw new TokenNotExpiredException();
            }

            var userDB = _userRepository.GetUserById(Convert.ToInt32(uniqueNameClaim.Value));
            return GenerateToken(userDB);

        }

        public UserDTO GetUserById(int userId, string authorizationHeader, bool IsTest = false)
        {
            string tokenString = IsTest ? authorizationHeader : authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");
            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Role");

            if (roleClaim.Value == Roles.User && Convert.ToInt32(uniqueNameClaim.Value) != userId)
            {
                _logger.LogWarning($"user with Id {uniqueNameClaim.Value} attempted to access information about user with id {userId}");
                throw new UnauthorizedAccessException("You dont have permission to retrieve this data");
            }

            var user = _userRepository.GetUserById(userId);
            _logger.LogInformation($"data abaout user with id {userId} retrieved by user with id {uniqueNameClaim.Value}");
            return _userRepository.GetUserById(userId).Adapt<UserDTO>();
        }
    }
}
