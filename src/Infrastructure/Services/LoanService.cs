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

            if (loan == null)
            {
                return "notFound";
            }


            if (roleClaim.Value == Roles.User && loan.UserId != Convert.ToInt32(uniqueNameClaim.Value))
            {
                _logger.LogWarning(
                    $"user with id {uniqueNameClaim.Value} attempted to delete loan with id {loan.Id}. " +
                    $"Request failed because this loan belongs to user with id {loan.UserId}"
                    );
                return null;
            }

            if (roleClaim.Value == Roles.User && loan.Status != LoanStatuses.Pending)
            {
                _logger.LogInformation($"deleting loan with id {loan.Id} failed because it`s status is not pending anmore");
                return "StatusFail";
            }

            _logger.LogInformation($"loan with id {loan.Id} is deleted from database");
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
                var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");

                if (bool.Parse(IsBlockedClaim.Value))
                {
                    _logger.LogInformation($"request loan failed because user with id ${uniqueNameClaim.Value} is blocked by accountant");
                    return "Blocked";
                }

                var emailClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Email");
                var adapdetLoan = requestDTO.Adapt<Loan>();
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

        public string UpdateLoan(int loanId, LoanRequestDTO loanDTO, string authorizationHeader)
        {
            string tokenString = authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Role");
            var uniqueNameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");
            var oldLoan = _loanRepository.GetLoanById(loanId);

            if (oldLoan == null)
            {
                return "notFound";
            }

            if (roleClaim.Value == Roles.User && Convert.ToInt32(uniqueNameClaim.Value) != oldLoan.UserId)
            {
                _logger.LogInformation($"Attemp to update loan with id {loanId} failed beacause user with id {uniqueNameClaim.Value} do not have permission");
                return null;
            }

            if (oldLoan.Status != LoanStatuses.Pending)
            {
                _logger.LogInformation($"updating loan with id {loanId} failed because it`s status is not pending anmore");
                return "statusFail";
            }

            var newLoan = loanDTO.Adapt<Loan>();
            newLoan.UserId = oldLoan.UserId;
            newLoan.Id = loanId;
            newLoan.Status = oldLoan.Status;
            _loanRepository.UpdateLoan(newLoan);
            _logger.LogInformation($"loan with id {loanId} is updated successfully");
            return "success";
        }
    }
}
