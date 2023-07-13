using Application.Accountant.Validators;
using Domain.DTO.Authentication;
using Domain.DTO.Loan;
using Domain.Interface;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                var result = _accountantService.UpdateUser(id, userDto, authorizationHeader);
                return Ok("User updated successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (DataNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
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

            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                _accountantService.UpdateLoan(id, loanDto, authorizationHeader);
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
        }
    }
}
