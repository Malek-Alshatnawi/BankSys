using BankSys.DTOs;
using BankSys.Models;
using BankSys.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BankSys.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerContrller : ControllerBase
    {
        CustomerRepository repo;

        public CustomerContrller(CustomerRepository repo)
        {
            this.repo = repo;
        }


        [HttpGet]
        [SwaggerOperation(summary: "Here you can get the customers With HowMany Accounts They Have", description: "")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var r = repo.GetCustomersWithNumOfAcc();
            return Ok(new
            {
                success = true,
                message = "The customer List feched successfully",
                data = r
            });
        }


        [HttpGet("{id}")]
        [SwaggerOperation(summary: "Here you can get the Customer By Id With His Accounts Count And His Total Balance InAll His Accounts", description: "")]
        public async Task<IActionResult> GetCustomerByID(int id)
        {
            var r = await repo.GetCustomerAndAccCountAndTotalBalance(id);
            if (r == null) return NotFound($"There is no customer with id {id}");
            else return Ok(new
            {
                success = true,
                message = "The custoemr detailes fetched successfully",
                data = r

            });
        }




        [HttpGet("{name}")]
        [SwaggerOperation(summary: "Here you can get the Customer By any name you think his Full name contains  With His Accounts Count And His Total Balance InAll His Accounts", description: "")]
        public async Task<IActionResult> GetCustomerNameContains(String name)
        {
            var r = await repo.GetCustomerAndAccCountAndTotalBalance(name);
            if (r == null) return NotFound($"There is no customer name contains  {name}");
            else return Ok(new
            {
                success = true,
                message = "The custoemr detailes fetched successfully",
                data = r

            });
        }


        [HttpGet("{customerid}")]
        [SwaggerOperation(summary: "Here you can get the Customer name&id along with a list of his accounts deyails, searching by custoemr id", description: "")]
        public async Task<IActionResult> GetCustomerWithAccDetailsByID(int customerid)
        {
            var r = await repo.GetCustomerWithAccountsByID(customerid);
            if (r == null) return NotFound($"There is no customer id with id {customerid}");
            else return Ok(new
            {
                succes = true,
                message = $"The customer details with id {customerid} feched successfully",
                data = r
            });

        }

        [HttpDelete("{customerid}")]
        [SwaggerOperation(summary: "Here you can delete a specific customer by id ", description: "Please make sure that the provided custmoer id doesn't have accounts")]
        public async Task<IActionResult> DeleteCustomerById(int customerid)
        {
            var r = await repo.DeleteCustomerById(customerid);
            if (r == "The mentioned customer id is not exist") return NotFound("The mentioned customer id is not exist");
            if (r == "The mentioned customer id has accounts  can't be deleted") return BadRequest("The mentioned customer id has accounts  can't be deleted");
            else return Ok(new { issuccess = true, message = "The customer has been deleted successfully" });

        }



        [HttpPost]
        [SwaggerOperation(summary: "Here you can add a new  customer", description: "")]
        public async Task<IActionResult> AddNewCustomer(CreateCustomerDTO cr)
        {
            var r = await repo.CreateCustomer(cr);
            if (r == "The customer added succssffully") return Ok("The customer added succssffully");
            else
                return null;
        }


        [HttpGet]
        [SwaggerOperation(summary: "Here you can fetch the an online products", description: "")]
        public async Task<IActionResult> FetchOnlineproducts()
        {
            var r = await repo.Importproducts();
             return Ok(new
            {
                success = true,
                message = "The products feched successfully",
            });


        }
    }
}
 