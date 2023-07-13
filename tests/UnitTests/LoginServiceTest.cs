using Domain.DTO.Authentication;
using Domain.Entities;
using Domain.Helpers;
using Domain.Interface;
using Infrastructure.Exceptions;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UnitTests
{
    [TestClass]
    public class LoginServiceTest
    {
        private AppSettings _appSettings = new AppSettings() { Secret = "random secret for jwt token" };
        private Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
        private Mock<ILogger<ILoginService>> _loggerMock = new Mock<ILogger<ILoginService>>();

        [TestMethod]
        public void Authenticate_Success()
        {
            _mockUserRepository.Setup(mock => mock.GetUserByUsername(It.IsAny<string>())).Returns(new User()
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG",
                FirstName = "test",
                LastName = "test",
                Email = "test",
                Role = Roles.User,
                IsBlocked = false,
                Verified = true
            });

            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);
            var result = loginService.Authenticate(new LoginDTO()
            {
                Username = "admin",
                Password = "admin"
            });

            Assert.IsTrue(IsTokenValid(result));
        }

        [TestMethod]
        [ExpectedException(typeof(WrongCredentialsException))]
        public void Authenticate_WrongUsername()
        {
            _mockUserRepository.Setup(mock => mock.GetUserByUsername(It.IsAny<string>())).Returns(new User()
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG",
                FirstName = "test",
                LastName = "test",
                Email = "test",
                Role = Roles.User,
                IsBlocked = false,
                Verified = true
            });

            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);
            var result = loginService.Authenticate(new LoginDTO()
            {
                Username = "admin",
                Password = "wrongPassword"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(WrongCredentialsException))]
        public void Authenticate_WrongPassword()
        {
            _mockUserRepository.Setup(mock => mock.GetUserByUsername(It.IsAny<string>())).Returns((User)null);

            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);
            loginService.Authenticate(new LoginDTO()
            {
                Username = "wrongusername",
                Password = "admin"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(UserUnverifiedException))]
        public void Authenticate_Unverified()
        {
            _mockUserRepository.Setup(mock => mock.GetUserByUsername(It.IsAny<string>())).Returns(new User()
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG",
                FirstName = "test",
                LastName = "test",
                Email = "test",
                Role = Roles.User,
                IsBlocked = false,
                Verified = false
            });

            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);
            loginService.Authenticate(new LoginDTO()
            {
                Username = "admin",
                Password = "admin"
            });
        }



        [TestMethod]
        public void Refreshtoken_Success()
        {
            _mockUserRepository.Setup(mock => mock.GetUserById(It.IsAny<int>())).Returns(new User()
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG",
                FirstName = "test",
                LastName = "test",
                Email = "test",
                Role = Roles.User,
                IsBlocked = false,
                Verified = true
            });
            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);
            var result = loginService.RefreshToken(GetToken());

            Assert.IsTrue(IsTokenValid(result));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Refreshtoken_InvalidToken()
        {
            _mockUserRepository.Setup(mock => mock.GetUserById(It.IsAny<int>())).Returns(new User()
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG",
                FirstName = "test",
                LastName = "test",
                Email = "test",
                Role = Roles.User,
                IsBlocked = false,
                Verified = true
            });
            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);
            loginService.RefreshToken("Invalid token");
        }

        [TestMethod]
        public void GetUserById_Success()
        {
            _mockUserRepository.Setup(mock => mock.GetUserById(It.IsAny<int>())).Returns(new User()
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG",
                FirstName = "test",
                LastName = "test",
                Email = "test",
                Role = Roles.User,
                IsBlocked = false,
                Verified = true
            });
            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);

            var result = loginService.GetUserById(1, GetToken(false), true);

            Assert.IsInstanceOfType<UserDTO>(result);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void GetUserById_Fail()
        {
            _mockUserRepository.Setup(mock => mock.GetUserById(It.IsAny<int>())).Returns(new User()
            {
                Id = 2,
                Username = "admin",
                Password = "$2a$10$s8B50T98cHcd.QNt2Zsv/eED6FzLmlKe/yxGgu8zLb0m5SmoO4xkG",
                FirstName = "test",
                LastName = "test",
                Email = "test",
                Role = Roles.User,
                IsBlocked = false,
                Verified = true
            });
            var loginService = new LoginService(_appSettings, _mockUserRepository.Object, _loggerMock.Object);
            loginService.GetUserById(2, GetToken(false), true);
        }


        private bool IsTokenValid(string tokenString)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(tokenString);
                return DateTime.UtcNow <= token.ValidTo;
            }
            catch
            {
                return false;
            }

        }

        private string GetToken(bool isAccoutant = true, bool isUserBlocked = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Role", isAccoutant ? "Accountant" : "User"),
                    new Claim(ClaimTypes.Name, "1"),
                    new Claim("IsBlocked", isAccoutant ? "false" : isUserBlocked.ToString()),
                    new Claim("Email", "test@email.com")
                }),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
