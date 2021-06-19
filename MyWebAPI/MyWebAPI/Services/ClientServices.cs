using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebModels.Database;
using MyWebModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Services
{
    public interface IClientServices
    {
        Task<ActionResult<IEnumerable<Client>>> GetAll();
        Client Find(int id);
        Task<ActionResult<bool>> Add(Client client);
        Task<ActionResult<bool>> Update(Client client);
        Task<ActionResult<bool>> Delete(int id);
        Task<bool> IsExists(int id);
    }
    public class ClientServices : IClientServices
    {
        private readonly AppDbContext context;

        public ClientServices(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ActionResult<bool>> Add(Client client)
        {
            context.Add(client);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<ActionResult<bool>> Delete(int id)
        {
            context.Remove(Find(id));
            await context.SaveChangesAsync();

            return true;
        }

        public Client Find(int id)
        {
            return context.Clients
                .Include(x => x.GetBills)
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task<ActionResult<IEnumerable<Client>>> GetAll()
        {
            return await context.Clients
                .Include(x => x.GetBills)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<bool> IsExists(int id)
        {
            return await context.Clients.AnyAsync(x => x.Id == id);
        }

        public async Task<ActionResult<bool>> Update(Client client)
        {
            context.Update(client);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
