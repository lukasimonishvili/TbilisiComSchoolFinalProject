using Application.Loans.Validators;
using Domain.DTO.Loan;
using Domain.Interface;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Application.Loans.Commands
{
    [Authorize]
    [ApiController]
    [Route("/api/[Controller]")]
    public class Loan : Controller
    {
        private readonly ILoanService _loanService;
        public Loan(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost("Request")]
        public IActionResult RequestLoan([FromBody] LoanRequestDTO loanRequestDTO)
        {
            var validator = new LoanRequestValidator().Validate(loanRequestDTO);
            if (!validator.IsValid)
            {
                var message = validator.Errors.Count > 1 ? "More then 1 validation error detected" : validator.Errors[0].ErrorMessage;
                return BadRequest(message);
            }

            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                _loanService.RequestLoan(loanRequestDTO, authorizationHeader);
                return Ok("Loan requested successfully. our accountant will review your request as soon as possible");
            }
            catch (UserBlockedEcxeption ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("GetByUserId/{id}")]
        public IActionResult GetLoansByUserId(int id)
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                var result = _loanService.GetLoansByUserId(id, authorizationHeader);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteLoanById(int id)
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                _loanService.DeleteLoanById(id, authorizationHeader);
                return Ok("Loan deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (DataNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (LoanStatusException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateLoan([FromBody] LoanRequestDTO loanRequestDTO, int id)
        {
            var validator = new LoanRequestValidator().Validate(loanRequestDTO);
            if (!validator.IsValid)
            {
                var message = validator.Errors.Count > 1 ? "More then 1 validation error detected" : validator.Errors[0].ErrorMessage;
                return BadRequest(message);
            }

            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                var result = _loanService.UpdateLoan(id, loanRequestDTO, authorizationHeader);
                return Ok("Loan updated successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (DataNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (LoanStatusException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
