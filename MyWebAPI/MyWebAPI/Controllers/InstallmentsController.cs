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
    public class InstallmentsController : ControllerBase
    {
        private readonly IInstallmentServices services;

        public InstallmentsController(IInstallmentServices services)
        {
            this.services = services;
        }

        // GET: api/Installments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Installment>>> GetAll()
        {
            return await services.GetAll();
        }

        // GET: api/Installments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Installment>>> GetAllReceived()
        {
            return await services.GetAllReceived();
        }

        // GET: api/Installments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Installment>>> GetAllUnreceived()
        {
            return await services.GetAllUnreceived();
        }

        // GET: api/Installments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Installment>>> GetAllToday()
        {
            return await services.GetAllToday();
        }

        // GET: api/Installments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Installment>>> GetReceivedToday()
        {
            return await services.GetReceivedToday();
        }

        // GET: api/Installments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Installment>>> GetUnreceivedToday()
        {
            return await services.GetUnreceivedToday();
        }

        // GET: api/Installments
        [HttpGet("{dateTime}")]
        public async Task<ActionResult<IEnumerable<Installment>>> GetByDay(string dateTime)
        {
            return await services.GetByDay(Convert.ToDateTime(dateTime));
        }

        // GET: api/Installments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Installment>> GetInstallment(int id)
        {
            var installment = services.Find(id);

            if (installment == null)
            {
                return await Task.FromResult(NotFound());
            }

            return installment;
        }

        // PUT: api/installment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstallment(int id, Installment installment)
        {
            if (id != installment.Id)
            {
                return BadRequest();
            }


            try
            {
                await services.Update(installment);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }
}
