using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebAPI.Services;
using MyWebModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly IBillServices services;

        public BillsController(IBillServices services)
        {
            this.services = services;
        }

        // GET: api/Bills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills()
        {
            return await services.GetAll();
        }

        // GET: api/Bills/3
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Bill>>> GetClientBills(int clientId)
        {
            return await services.GetAll(clientId);
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

            return NoContent();
        }

        // POST: api/Bills
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(Bill bill)
        {
            await services.Add(bill);

            return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
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

            return NoContent();
        }
    }
}
