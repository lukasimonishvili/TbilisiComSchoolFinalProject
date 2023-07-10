using Application.Loans.Validators;
using Domain.DTO.Loan;
using Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var result = _loanService.RequestLoan(loanRequestDTO, authorizationHeader);

            if (result == null)
            {
                return Unauthorized("Invalid Token");
            }

            if (result == "Blocked")
            {
                return Conflict("Your account is temporary blocked for new loan requests. please contact your accountant");
            }

            return Ok("Loan requested successfully. our accountant will review your request as soon as possible");
        }

        [HttpGet("GetByUserId/{id}")]
        public IActionResult GetLoansByUserId(int id)
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var result = _loanService.GetLoansByUserId(id, authorizationHeader);

            if (result == null)
            {
                return Unauthorized("You dont have permission to retrieve these data");
            }

            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteLoanById(int id)
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var result = _loanService.DeleteLoanById(id, authorizationHeader);

            if (result == null)
            {
                return Unauthorized("You dont have permission to delete this loan");
            }

            if (result == "notFound")
            {
                return NotFound($"loan with id {id} not found in system");
            }

            if (result == "StatusFail")
            {
                return BadRequest("You cant delete confirmed or rejected loans");
            }

            return Ok("Loan deleted successfully");
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

            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var result = _loanService.UpdateLoan(id, loanRequestDTO, authorizationHeader);

            if (result == null)
            {
                return Unauthorized("You dont have permission to update this loan");
            }

            if (result == "notFound")
            {
                return NotFound($"loan with id {id} not found in system");
            }

            if (result == "StatusFail")
            {
                return BadRequest("You cant update confirmed or rejected loans");
            }

            return Ok("Loan updated successfully");
        }
    }
}
