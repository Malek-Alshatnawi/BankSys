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
        public LoanController(LoanRepo _repo) { 
            repo = _repo;
        }


        [HttpPost]
        [SwaggerOperation(summary:"Here you can apply for a loan",description:"please make sure that the customer id is exist and the loan amount is not more than 10x monyhly income")]
        public async Task<IActionResult> LoanApplication(LoanApplicationRequest LA)
        {
            var r = await repo.LoanApplication(LA);
            if (r == ENUMs.LoanApplicationResults.TheprovidedCustomerIdIsNotExist) return NotFound("The provided customer id is not exist");
            if (r == ENUMs.LoanApplicationResults.Loanamountexceedsallowedlimit) return BadRequest("Loan amount exceeds allowed limit");
            return Ok(new { 
                issuccess = true,
                message = "The Loan Application Is Approved"
            });

        }

    }
}
 