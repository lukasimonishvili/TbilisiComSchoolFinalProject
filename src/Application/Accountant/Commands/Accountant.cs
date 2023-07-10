using Application.Accountant.Validators;
using Domain.DTO.Authentication;
using Domain.DTO.Loan;
using Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Accountant.Commands
{
    [Authorize]
    [ApiController]
    [Route("/api/[Controller]")]
    public class Accountant : Controller
    {
        private readonly IAccountantService _accountantService;

        public Accountant(IAccountantService accountantService)
        {
            _accountantService = accountantService;
        }

        [HttpPost("UpdateUser/{id}")]
        public IActionResult UpdateUser([FromBody] UserAccountantDTO userDto, int id)
        {
            var validator = new UserAccountantValidator().Validate(userDto);
            if (!validator.IsValid)
            {
                var message = validator.Errors.Count > 1 ? "More then 1 validation error detected" : validator.Errors[0].ErrorMessage;
                return BadRequest(message);
            }
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var result = _accountantService.UpdateUser(id, userDto, authorizationHeader);

            if (result == null)
            {
                return Unauthorized("Permision denied");
            }

            if (result == "notFound")
            {
                return NotFound($"user with id {id} not found in system");
            }

            return Ok("User updated successfully");
        }

        [HttpPost("UpdateLoan/{id}")]
        public IActionResult UpdateLoan([FromBody] LoanAccountantDTO loanDto, int id)
        {
            var validator = new LoanAccountantValidator().Validate(loanDto);
            if (!validator.IsValid)
            {
                var message = validator.Errors.Count > 1 ? "More then 1 validation error detected" : validator.Errors[0].ErrorMessage;
                return BadRequest(message);
            }

            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            var result = _accountantService.UpdateLoan(id, loanDto, authorizationHeader);

            if (result == null)
            {
                return Unauthorized("Permision denied");
            }

            if (result == "notFound")
            {
                return NotFound($"loan with id {id} not found in system");
            }

            return Ok("Loan updated successfully");
        }
    }
}
