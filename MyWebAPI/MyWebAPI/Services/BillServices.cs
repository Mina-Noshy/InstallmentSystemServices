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
    public interface IBillServices
    {
        Task<ActionResult<IEnumerable<Bill>>> GetAll();
        Task<ActionResult<IEnumerable<Bill>>> GetAll(int clientId);
        Bill Find(int billId);
        Task<ActionResult<bool>> Add(Bill bill);
        Task<ActionResult<bool>> Update(Bill bill);
        Task<ActionResult<bool>> Delete(int billId);
        Task<bool> IsExists(int billId);
    }
    public class BillServices : IBillServices
    {
        private readonly AppDbContext context;

        public BillServices(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ActionResult<bool>> Add(Bill bill)
        {
            double _amountValue = bill.RestAmount / bill.InstallmentCount;

            context.Add(bill);
            await context.SaveChangesAsync();

            for (int i = 1; i <= bill.InstallmentCount; i++)
            {
                if (bill.InstallmentType == InstallmentTypes.سنوى)
                {
                    context.Add(new Installment
                    {
                        BillId = bill.Id,
                        AmountValue = _amountValue,
                        DueDate = DateTime.Now.Date.AddYears(i)
                    });
                }
                else if (bill.InstallmentType == InstallmentTypes.شهرى)
                {
                    context.Add(new Installment
                    {
                        BillId = bill.Id,
                        AmountValue = _amountValue,
                        DueDate = DateTime.Now.Date.AddMonths(i)
                    });
                }
                else
                {
                    context.Add(new Installment
                    {
                        BillId = bill.Id,
                        AmountValue = _amountValue,
                        DueDate = DateTime.Now.Date.AddDays(i)
                    });
                }

            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<ActionResult<bool>> Delete(int billId)
        {
            context.Remove(Find(billId));
            await context.SaveChangesAsync();

            return true;
        }

        public Bill Find(int billId)
        {
            return context.Bills
                .Include(x => x.GetClient)
                .Include(x => x.GetInstallments)
                .FirstOrDefault(x => x.Id == billId);
        }

        public async Task<bool> IsExists(int billId)
        {
            return await context.Bills.AnyAsync(x => x.Id == billId);
        }

        public async Task<ActionResult<bool>> Update(Bill bill)
        {
            context.Update(bill);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<ActionResult<IEnumerable<Bill>>> GetAll()
        {
            return await context.Bills
                .Include(x => x.GetClient)
                .Include(x => x.GetInstallments)
                .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Bill>>> GetAll(int clientId)
        {
            return await context.Bills
                .Include(x => x.GetClient)
                .Include(x => x.GetInstallments)
                .Where(x => x.ClientId == clientId)
                .ToListAsync();
        }
    }
}
