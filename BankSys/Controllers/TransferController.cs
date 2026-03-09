using BankSys.DTOs;
using BankSys.Enums;
using BankSys.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace BankSys.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        TransferRepository repo;

        public TransferController(TransferRepository repo)
        {
            this.repo = repo;
        }



        [HttpPost]
        [SwaggerOperation(summary: "Here you can reverse a trasnfer from acc to antother", description: "please make sure that the transfer is already exist")]
        public async Task<IActionResult> RevReverseATransfer (ReverseTransferDTO rt)
        {
            if (rt== null) return BadRequest ("Please provide the transfer details");
            var result = await repo.ReverseATransfer(rt);
            if (result == ReverseTransferResults.ThereIsNoTransferWithTheprovidedDetailes) return NotFound("There is no transfer with the provided details");
            if (result == ReverseTransferResults.InsufciantBalance) return BadRequest("The reversal cannot be completed due to insufficient balance in the account");
            return Ok("The reversal completed successfully");
        }


    }
}
