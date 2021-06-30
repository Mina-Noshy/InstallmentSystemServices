using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebModels.Database;
using MyWebModels.Models;
using MyWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Services
{
    public interface IClientServices
    {
        Task<ActionResult<IEnumerable<ClientMobileVM>>> GetAllVM(string userId);
        Task<ActionResult<IEnumerable<ClientMobileVM>>> GetAllVM(string userId, string txt);
        ClientVM FindVM(int id);
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
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task<ActionResult<IEnumerable<ClientMobileVM>>> GetAllVM(string userId)
        {
            return await context.Clients
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Name)
                .Select(x => new ClientMobileVM
                {
                    id = x.Id,
                    address = x.AddressDetails,
                    phone = x.Phone_1
                }).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<ClientMobileVM>>> GetAllVM(string userId, string txt)
        {
            return await context.Clients
                .Where(x => x.UserId == userId && x.Name.Contains(txt))
                .OrderBy(x => x.Name)
                .Select(x => new ClientMobileVM
                {
                    id = x.Id,
                    address = x.AddressDetails,
                    phone = x.Phone_1
                }).ToListAsync();
        }


        public ClientVM FindVM(int id)
        {
            return context.Clients
                .Select(x => new ClientVM
                {
                    id = x.Id,
                    name = x.Name,
                    address = x.AddressDetails,
                    email = x.Email,
                    fax = x.Fax,
                    phone_1 = x.Phone_1,
                    phone_2 = x.Phone_2,
                    phone_3 = x.Phone_3
                })
                .FirstOrDefault(x => x.id == id);
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
