using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebAPI.Services;
using MyWebModels.Models;
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
    public class InstallmentsController : ControllerBase
    {
        private readonly IInstallmentServices services;

        public InstallmentsController(IInstallmentServices services)
        {
            this.services = services;
        }


        // GET: api/Installments
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetAllUnreceivedVM(string userId)
        {
            return await services.GetAllUnreceivedVM(userId);
        }

        // GET: api/Installments
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetAllTodayVM(string userId)
        {
            return await services.GetAllTodayVM(userId);
        }

        // GET: api/Installments
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetReceivedTodayVM(string userId)
        {
            return await services.GetReceivedTodayVM(userId);
        }

        // GET: api/Installments
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetUnreceivedTodayVM(string userId)
        {
            return await services.GetUnreceivedTodayVM(userId);
        }

        // GET: api/Installments
        [HttpGet("{userId}/{dateTime}")]
        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetByDayVM(string userId, string dateTime)
        {
            return await services.GetByDayVM(userId, Convert.ToDateTime(dateTime));
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

        // GET: api/Installments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InstallmentVM>> GetInstallmentVM(int id)
        {
            var installment = await services.FindVM(id);

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

            return Ok();
        }
    }
}
