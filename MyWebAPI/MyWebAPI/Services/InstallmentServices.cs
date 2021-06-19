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
    public interface IInstallmentServices
    {
        Task<ActionResult<IEnumerable<Installment>>> GetAll();
        Task<ActionResult<IEnumerable<Installment>>> GetAllReceived();
        Task<ActionResult<IEnumerable<Installment>>> GetAllUnreceived();
        Task<ActionResult<IEnumerable<Installment>>> GetAllToday();
        Task<ActionResult<IEnumerable<Installment>>> GetReceivedToday();
        Task<ActionResult<IEnumerable<Installment>>> GetUnreceivedToday();
        Task<ActionResult<IEnumerable<Installment>>> GetByDay(DateTime dateTime);
        Task<ActionResult<bool>> Update(Installment installment);
        Installment Find(int id);
    }

    public class InstallmentServices : IInstallmentServices
    {
        private readonly AppDbContext context;

        public InstallmentServices(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ActionResult<IEnumerable<Installment>>> GetAll()
        {
            return await context.Installments
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .OrderBy(x => x.GetBill.ClientId)
                .ToListAsync();
        }
        public async Task<ActionResult<IEnumerable<Installment>>> GetAllReceived()
        {
            return await context.Installments
                .Where(x => x.ReceivedDate != null)
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .OrderBy(x => x.GetBill.ClientId)
                .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Installment>>> GetAllUnreceived()
        {
            return await context.Installments
                .Where(x => x.ReceivedDate == null)
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .OrderBy(x => x.GetBill.ClientId)
                .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Installment>>> GetAllToday()
        {
            return await context.Installments
                .Where(x => x.DueDate.Date == DateTime.UtcNow.Date)
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Installment>>> GetReceivedToday()
        {
            return await context.Installments
                .Where(x => x.DueDate.Date == DateTime.UtcNow.Date && x.ReceivedDate != null)
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Installment>>> GetUnreceivedToday()
        {
            return await context.Installments
               .Where(x => x.DueDate.Date == DateTime.UtcNow.Date && x.ReceivedDate == null)
               .Include(m => m.GetBill)
               .ThenInclude(m => m.GetClient)
               .OrderBy(x => x.DueDate)
               .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Installment>>> GetByDay(DateTime dateTime)
        {
            return await context.Installments
               .Where(x => x.DueDate.Date == dateTime.Date)
               .Include(m => m.GetBill)
               .ThenInclude(m => m.GetClient)
               .OrderBy(x => x.DueDate)
               .ToListAsync();
        }

        public async Task<ActionResult<bool>> Update(Installment installment)
        {

            if (installment.ReceivedDate == null)
                installment.ReceivedDate = DateTime.UtcNow;
            else
                installment.ReceivedDate = null;

            context.Update(installment);
            await context.SaveChangesAsync();
            return true;

        }

        public Installment Find(int id)
        {
            return context.Installments
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
