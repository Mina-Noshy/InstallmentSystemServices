using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebAPI.Services;
using MyWebModels.Models;
using MyWebModels.Models.Account;
using MyWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Moderator,User")]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly IBillServices services;
        private readonly UserManager<AppUser> userManager;

        public BillsController(IBillServices services, UserManager<AppUser> userManager)
        {
            this.services = services;
            this.userManager = userManager;
        }

        // GET: api/Bills
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<BillMobileVM>>> GetUserBillsVM(string userId)
        {
            return await services.GetAllVM(userId);
        }

        // GET: api/Bills/3
        [HttpGet("{clientId}")]
        public async Task<ActionResult<IEnumerable<BillMobileVM>>> GetClientBillsVM(int clientId)
        {
            return await services.GetAllVM(clientId);
        }

        // GET: api/Bills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
            var bill = services.Find(id);

            if (bill == null)
            {
                return await Task.FromResult(NotFound());
            }

            return bill;
        }

        // GET: api/Bills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillVM>> GetBillVM(int id)
        {
            var bill = services.FindVM(id);

            if (bill == null)
            {
                return await Task.FromResult(NotFound());
            }

            return bill;
        }

        // PUT: api/Bills/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill(int id, Bill bill)
        {
            if (id != bill.Id)
            {
                return BadRequest();
            }


            try
            {
                await services.Update(bill);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await services.IsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Bills
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(Bill bill)
        {
            await services.Add(bill);

            return Ok();
        }

        // DELETE: api/Bills/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var bill = services.Find(id);
            if (bill == null)
            {
                return NotFound();
            }

            await services.Delete(id);

            return Ok();
        }
    }
}
