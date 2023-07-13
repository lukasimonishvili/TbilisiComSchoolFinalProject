using Domain.DTO.Loan;
using Domain.Entities;
using Domain.Interface;
using Infrastructure.Exceptions;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UnitTests
{
    [TestClass]
    public class LoanServiceTest
    {
        private Mock<ILoanRepository> _mockLoanRepo = new Mock<ILoanRepository>();
        private Mock<ILogger<ILoanService>> _mockLogger = new Mock<ILogger<ILoanService>>();

        [TestMethod]
        public void DeleteLoanById_Result_Okay_User()
        {

            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 1,
                Id = 1,
                Status = LoanStatuses.Pending
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            var response = loanService.DeleteLoanById(1, GetToken(isAccoutant: false), true);

            Assert.AreEqual("Success", response);
        }

        [TestMethod]
        public void DeleteLoanById_Result_Okay_Accountant()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 2,
                Id = 1,
                Status = LoanStatuses.Denied
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            var response = loanService.DeleteLoanById(1, GetToken(), true);

            Assert.AreEqual("Success", response);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void DeleteLoanById_Result_noPermision()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 2,
                Id = 1,
                Status = LoanStatuses.Denied
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.DeleteLoanById(1, GetToken(isAccoutant: false), true);
        }

        [TestMethod]
        [ExpectedException(typeof(DataNotFoundException))]
        public void DeleteLoanById_Loan_Not_Found_User()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns((Loan)null);

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.DeleteLoanById(1, GetToken(isAccoutant: false), true);
        }

        [TestMethod]
        [ExpectedException(typeof(LoanStatusException))]
        public void DeleteLoanById_StatusFail()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 1,
                Id = 1,
                Status = LoanStatuses.Denied
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.DeleteLoanById(1, GetToken(isAccoutant: false), true);
        }

        [TestMethod]
        public void GetLoansByUserId_Returns_Okay()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoansByUsrId(It.IsAny<int>())).Returns(new List<Loan>());

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            var response = loanService.GetLoansByUserId(1, GetToken(isAccoutant: false), true);

            Assert.IsInstanceOfType<List<LoanDTO>>(response);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void GetLoansByUserId_Unauthorized()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoansByUsrId(It.IsAny<int>())).Returns(new List<Loan>());

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.GetLoansByUserId(2, GetToken(isAccoutant: false), true);
        }

        [TestMethod]
        public void RequestLoan_Returns_Okay()
        {
            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            var response = loanService.RequestLoan(new LoanRequestDTO(), GetToken(isAccoutant: false), true);

            Assert.AreEqual("Success", response);
        }

        [TestMethod]
        [ExpectedException(typeof(UserBlockedEcxeption))]
        public void RequestLoan_BlockedUser()
        {
            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.RequestLoan(new LoanRequestDTO(), GetToken(isAccoutant: false, isUserBlocked: true), true);
        }


        [TestMethod]
        public void UpdateLoanById_Result_Okay_User()
        {

            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 1,
                Id = 1,
                Status = LoanStatuses.Pending
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            var response = loanService.UpdateLoan(1, new LoanRequestDTO(), GetToken(isAccoutant: false), true);

            Assert.AreEqual("success", response);
        }

        [TestMethod]
        public void UpdateLoanById_Result_Okay_Accountant()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 2,
                Id = 1,
                Status = LoanStatuses.Denied
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            var response = loanService.UpdateLoan(1, new LoanRequestDTO(), GetToken(), true);

            Assert.AreEqual("success", response);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void UpdateLoanById_Result_noPermision()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 2,
                Id = 1,
                Status = LoanStatuses.Denied
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.UpdateLoan(1, new LoanRequestDTO(), GetToken(isAccoutant: false), true);
        }

        [TestMethod]
        [ExpectedException(typeof(DataNotFoundException))]
        public void UpdateLoanById_Loan_Not_Found()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns((Loan)null);

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.UpdateLoan(1, new LoanRequestDTO(), GetToken(isAccoutant: false), true);
        }

        [TestMethod]
        [ExpectedException(typeof(LoanStatusException))]
        public void UpdateLoanById_StatusFail()
        {
            _mockLoanRepo.Setup(mock => mock.GetLoanById(It.IsAny<int>())).Returns(new Loan()
            {
                UserId = 1,
                Id = 1,
                Status = LoanStatuses.Denied
            });

            var loanService = new LoanService(_mockLoanRepo.Object, _mockLogger.Object);
            loanService.UpdateLoan(1, new LoanRequestDTO(), GetToken(isAccoutant: false), true);
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
