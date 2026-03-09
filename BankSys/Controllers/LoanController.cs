using BankSys.DTOs;
using BankSys.Enums;
using LoanSys.DTOs;
using LoanSys.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoanSys.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        LoanRepo repo;
        public LoanController(LoanRepo _repo)
        {
            repo = _repo;
        }


        [HttpPost]
        [SwaggerOperation(summary: "Here you can apply for a loan", description: "please make sure that the customer id is exist and the loan amount is not more than 10x monyhly income")]
        public async Task<IActionResult> LoanApplication(LoanApplicationRequest LA)
        {
            var r = await repo.LoanApplication(LA);
            if (r == ENUMs.LoanApplicationResults.TheprovidedCustomerIdIsNotExist) return NotFound("The provided customer id is not exist");
            if (r == ENUMs.LoanApplicationResults.Loanamountexceedsallowedlimit) return BadRequest("Loan amount exceeds allowed limit");

            return Ok(new
            {
                issuccess = true,
                message = "The Loan Application Is Pending"
            });

        }

        [HttpPut("{loanid}")]
        [SwaggerOperation(summary: "Here you can approve a Lone", description: "Please make sure that the loan status is pending ")]
        public async Task<IActionResult> ApproveLone(int loanid)
        {
            var r = await repo.ApproveLoan(loanid);
            if (r == ApproveLoanResults.ThereIsNoLoanWithThisId) return NotFound($"There is no loan with id {loanid}");
            if (r == ApproveLoanResults.ToApproveTheLoanTheStatusShouldBePending) return BadRequest($"To Approve The Loan The Status Should Be Pending");

            return Ok(new
            {
                issuccess = true,
                message = "The Loan Application has been approved"
            });
        }


        [HttpGet("{LoanId}")]
        [SwaggerOperation(summary: "Here you can get a specific loan with the loan installments and payments", description: "please search by loan id and make sure its exist")]
        public async Task<IActionResult> GetLoanById(int LoanId)
        {
            var r = await repo.GetLoanByID(LoanId);
            if (r == null) return NotFound($"There is no loan with id {LoanId}");
            return Ok(r);
        }


        [HttpPost]
        [SwaggerOperation(summary: "Here you can make a payment for a specific loan", description: "please make sure that the loan id is exist and the payment amount is not more than the remaining amount of the loan")]
        public async Task<IActionResult> MakePayment(MakepaymentRequest p )
        {
            var r = await repo.PayInstallemt(p);
            if (r == PayInstallemtResults.LoanNotFound) return NotFound($"There is no loan with id {p.LoanId}");
            if (r == PayInstallemtResults.InvalidAmount) return BadRequest("Payment amount is not matching the the due amount");
            if (r == PayInstallemtResults.ThereIsNoUnpaidInstallmetnsForThisLoan) return BadRequest("There is  no installemts for the mentioend loan");

            return Ok(new
            {
                issuccess = true,
                message = "The Payment has been made successfully"
            });


        }


    }
}
