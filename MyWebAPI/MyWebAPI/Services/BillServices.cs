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
    public interface IBillServices
    {
        Task<ActionResult<IEnumerable<BillMobileVM>>> GetAllVM(string userId);
        Task<ActionResult<IEnumerable<BillMobileVM>>> GetAllVM(int clientId);
        BillVM FindVM(int billId);
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

        public async Task<ActionResult<IEnumerable<BillMobileVM>>> GetAllVM(string userId)
        {
            return await context.Bills
                .Include(x => x.GetClient)
                .Where(x => x.GetClient.UserId == userId)
                .OrderBy(x => x.Id)
                .Select(x => new BillMobileVM
                {
                    id = x.Id,
                    client = x.GetClient.Name,
                    originalAmount = x.OriginalAmount,
                    amountPaid = x.AmountPaid,
                    percentage = x.Percentage,
                    restAmount = x.RestAmount,
                    totalAmount = x.TotalAmount
                }).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<BillMobileVM>>> GetAllVM(int clientId)
        {
            return await context.Bills
                .Include(x => x.GetClient)
                .Where(x => x.ClientId == clientId)
                .OrderBy(x => x.Id)
                .Select(x => new BillMobileVM
                {
                    id = x.Id,
                    client = x.GetClient.Name,
                    originalAmount = x.OriginalAmount,
                    amountPaid = x.AmountPaid,
                    percentage = x.Percentage,
                    restAmount = x.RestAmount,
                    totalAmount = x.TotalAmount
                }).ToListAsync();
        }

        public BillVM FindVM(int billId)
        {
            return context.Bills
                .Include(x => x.GetClient)
                .Select(x => new BillVM
                {
                    id = x.Id,
                    clientId = x.ClientId,
                    clientName = x.GetClient.Name,
                    originalAmount = x.OriginalAmount,
                    amountPaid = x.AmountPaid,
                    restAmount = x.RestAmount,
                    installmentCount = x.InstallmentCount,
                    percentage = x.Percentage,
                    totalAmount = x.TotalAmount,
                    billDate = x.BillDate,
                    delayFine = x.DelayFine,
                    description = x.Description,
                    delayFineType = x.DelayFineType,
                    installmentType = x.InstallmentType
                }).FirstOrDefault(x => x.id == billId);
        }


    }
}
