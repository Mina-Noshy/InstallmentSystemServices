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
    public class ClientsController : ControllerBase
    {
        private readonly IClientServices services;

        public ClientsController(IClientServices services)
        {
            this.services = services;
        }

        // GET: api/Clients
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ClientMobileVM>>> GetClientsVM(string userId)
        {
            return await services.GetAllVM(userId);
        }

        [HttpGet("{userId}/{txt}")]
        public async Task<ActionResult<IEnumerable<ClientMobileVM>>> SearchVM(string userId, string txt)
        {
            return await services.GetAllVM(userId, txt);
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = services.Find(id);

            if (client == null)
            {
                return await Task.FromResult(NotFound());
            }

            return client;
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientVM>> GetClientVM(int id)
        {
            var client = services.FindVM(id);

            if (client == null)
            {
                return await Task.FromResult(NotFound());
            }

            return client;
        }

        // PUT: api/Clients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }


            try
            {
                await services.Update(client);
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

        // POST: api/Clients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            await services.Add(client);

            return Ok();
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = services.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            await services.Delete(id);

            return Ok();
        }
    }
}
