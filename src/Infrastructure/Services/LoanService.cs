using Domain.DTO.Loan;
using Domain.Entities;
using Domain.Interface;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Infrastructure.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger<ILoanService> _logger;

        public LoanService(ILoanRepository loanRepository, ILogger<ILoanService> logger)
        {
            _loanRepository = loanRepository;
            _logger = logger;
        }

        public string DeleteLoanById(int loanId, string authorizationHeader)
        {
            string tokenString = authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Role");
            var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");
            var loan = _loanRepository.GetLoanById(loanId);

            if (roleClaim.Value == Roles.User && loan.UserId != Convert.ToInt32(uniqueNameClaim.Value))
            {
                return null;
            }

            if (roleClaim.Value == Roles.User && loan.Status != LoanStatuses.Pending)
            {
                return "StatusFail";
            }

            _loanRepository.DeleteLoan(loan);
            return "Success";
        }

        public List<LoanDTO> GetLoansByUserId(int userId, string authorizationHeader)
        {
            string tokenString = authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Role");
            var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");

            if (roleClaim.Value == Roles.User && userId != Convert.ToInt32(uniqueNameClaim.Value))
            {
                return null;
            }

            var LoanRepository = _loanRepository.GetLoansByUsrId(Convert.ToInt32(uniqueNameClaim.Value));
            var adapdetLoanList = LoanRepository.Adapt<List<LoanDTO>>();
            return adapdetLoanList;
        }

        public string RequestLoan(LoanRequestDTO requestDTO, string authorizationHeader)
        {
            try
            {
                string tokenString = authorizationHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(tokenString);
                var IsBlockedClaim = token.Claims.FirstOrDefault(claim => claim.Type == "IsBlocked");

                if (bool.Parse(IsBlockedClaim.Value))
                {
                    return "Blocked";
                }

                var emailClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Email");
                var adapdetLoan = requestDTO.Adapt<Loan>();
                var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");
                adapdetLoan.UserId = Convert.ToInt32(uniqueNameClaim.Value);
                adapdetLoan.Status = LoanStatuses.Pending;
                _loanRepository.AddLoanToDatabase(adapdetLoan);
                _logger.LogInformation($"New Loan Requested By User with email address - {emailClaim.Value}");
                return "Success";
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return null;
            }
        }
    }
}
