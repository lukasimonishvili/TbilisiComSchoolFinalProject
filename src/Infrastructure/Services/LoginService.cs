
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
using Infrastructure.Repositories;
using System.Linq;

namespace Infrastructure.Services
{
    public class LoginService : ILoginService
    {
        private readonly AppSettings _appSettings;
        private readonly UserRepository _userRepository;
        public LoginService(AppSettings appSettings, UserRepository userRepository)
        {
            _appSettings = appSettings;
            _userRepository = userRepository;
        }


        public string Authenticate(LoginDTO login)
        {
            var user = _userRepository.GetUserByUsername(login.Username);

            if (user == null)
            {
                return null;
            }

            if (!BCryptNet.Verify(login.Password, user.Password))
            {
                return null;
            }

            if (!user.Verified)
            {
                return "Unverified";
            }
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
                    new Claim("Role", user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        private bool IsTokenExpired(string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = jwtTokenHandler.ReadJwtToken(token);
            var expirationDate = decodedToken.ValidTo;
            var currentTime = DateTime.UtcNow;

            return expirationDate <= currentTime;
        }

        public string RefreshToken(string tokenString)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(tokenString);
                var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");

                if (!IsTokenExpired(tokenString))
                {
                    return "Valid";
                }
                else
                {
                    var userDB = _userRepository.GetUserById(Convert.ToInt32(uniqueNameClaim.Value));
                    return GenerateToken(userDB);
                }
            }
            catch
            {
                return null;
            }


        }
    }
}
