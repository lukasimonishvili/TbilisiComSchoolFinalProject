using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using Domain.Interface;
using Infrastructure.Services;
using Domain.DTO.Loan;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Domain.DTO.Authentication;
using System;
using Infrastructure.Exceptions;

namespace UnitTests
{
    [TestClass]
    public class AccountantServiceTest
    {
        private Mock<ILoanRepository> _mockLoanRepo;
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<ILogger<IAccountantService>> _mockLogger;

        [TestMethod]
        public void UpdateLoan_Response_Okay()
        {
            SetUpTestLoanData();

            var accountantService = new AccountantService(_mockUserRepo.Object, _mockLoanRepo.Object, _mockLogger.Object);
            var response = accountantService.UpdateLoan(1, new LoanAccountantDTO() { }, GetToken(), true);

            Assert.AreEqual("success", response);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void UpdateLoan_NotAccoutant()
        {
            SetUpTestLoanData();

            var accountantService = new AccountantService(_mockUserRepo.Object, _mockLoanRepo.Object, _mockLogger.Object);
            accountantService.UpdateLoan(1, new LoanAccountantDTO() { }, GetToken(isAccoutant: false), true);
        }

        [TestMethod]
        [ExpectedException(typeof(DataNotFoundException))]
        public void UpdateLoan_LoanNotFound_ReturnNotFound()
        {
            SetUpTestLoanData(isLoanNull: true);

            var accountantService = new AccountantService(_mockUserRepo.Object, _mockLoanRepo.Object, _mockLogger.Object);
            accountantService.UpdateLoan(1, new LoanAccountantDTO() { }, GetToken(), true);
        }

        [TestMethod]
        public void UpdateUser_Response_Okay()
        {
            SetUpUserTestData();
            var accountantService = new AccountantService(_mockUserRepo.Object, _mockLoanRepo.Object, _mockLogger.Object);
            var response = accountantService.UpdateUser(1, new UserAccountantDTO() { }, GetToken(), true);

            Assert.AreEqual("success", response);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void UpdatUser_NotAccoutant()
        {
            SetUpTestLoanData();

            var accountantService = new AccountantService(_mockUserRepo.Object, _mockLoanRepo.Object, _mockLogger.Object);
            accountantService.UpdateUser(1, new UserAccountantDTO() { }, GetToken(isAccoutant: false), true);
        }


        [TestMethod]
        [ExpectedException(typeof(DataNotFoundException))]
        public void UpdateUser_UserNotFound_ReturnNotFound()
        {
            SetUpTestLoanData(isLoanNull: true);

            var accountantService = new AccountantService(_mockUserRepo.Object, _mockLoanRepo.Object, _mockLogger.Object);
            accountantService.UpdateUser(1, new UserAccountantDTO() { }, GetToken(), true);
        }

        private void SetUpTestLoanData(bool isLoanNull = false)
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLoanRepo = new Mock<ILoanRepository>();
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(isLoanNull ? (Loan)null : new Loan()
            {
                UserId = 1,
                Id = 1,
            });
            _mockLogger = new Mock<ILogger<IAccountantService>>();
        }

        private void SetUpUserTestData(bool isUserNull = false)
        {
            _mockLoanRepo = new Mock<ILoanRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockUserRepo.Setup(mock => mock.GetUserById(It.IsAny<int>())).Returns(isUserNull ? (User)null : new User()
            {
                Id = 1
            });
            _mockLogger = new Mock<ILogger<IAccountantService>>();
        }


        private string GetToken(bool isAccoutant = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Role", isAccoutant ? "Accountant" : "User"),
                    new Claim(ClaimTypes.Name, "1")
                }),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
