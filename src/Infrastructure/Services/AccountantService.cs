using Domain.DTO.Authentication;
using Domain.DTO.Loan;
using Domain.Entities;
using Domain.Interface;
using Infrastructure.Exceptions;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Infrastructure.Services
{
    public class AccountantService : IAccountantService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger<IAccountantService> _logger;

        public AccountantService(IUserRepository userRepository, ILoanRepository loanRepository, ILogger<IAccountantService> logger)
        {
            _userRepository = userRepository;
            _loanRepository = loanRepository;
            _logger = logger;
        }

        public string UpdateLoan(int loanId, LoanAccountantDTO loanDto, string authorizationHeader, bool IsTest = false)
        {
            string tokenString = IsTest ? authorizationHeader : authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Role");
            var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");

            if (roleClaim.Value != Roles.Accountant)
            {
                _logger.LogWarning($"none admin user with id {uniqueNameClaim.Value} attempted to update loan data");
                throw new UnauthorizedAccessException("Permision denied");
            }

            var oldLoan = _loanRepository.GetLoanById(loanId);
            if (oldLoan == null)
            {
                throw new DataNotFoundException($"loan with id {loanId} not found in system");
            }

            var newLoan = loanDto.Adapt<Loan>();
            newLoan.UserId = oldLoan.UserId;
            newLoan.Id = oldLoan.Id;
            _loanRepository.UpdateLoan(newLoan);
            return "success";
        }

        public string UpdateUser(int userId, UserAccountantDTO userDto, string authorizationHeader, bool IsTest = false)
        {
            string tokenString = IsTest ? authorizationHeader : authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Role");
            var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");

            if (roleClaim.Value != Roles.Accountant)
            {
                _logger.LogWarning($"none admin user with id {uniqueNameClaim.Value} attempted to update user data");
                throw new UnauthorizedAccessException("Permision denied");
            }

            var oldUser = _userRepository.GetUserById(userId);
            if (oldUser == null)
            {
                throw new DataNotFoundException($"loan with id {userId} not found in system");
            }

            var newUser = userDto.Adapt<User>();
            newUser.Id = userId;
            newUser.Password = oldUser.Password;
            _userRepository.UpdateUser(newUser);
            return "success";
        }


    }
}
