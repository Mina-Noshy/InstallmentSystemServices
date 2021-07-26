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
    public interface IInstallmentServices
    {
        Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetBillInstallmentsVM(int billId);
        Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetAllUnreceivedVM(string userId);
        Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetAllTodayVM(string userId);
        Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetReceivedTodayVM(string userId);
        Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetUnreceivedTodayVM(string userId);
        Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetByDayVM(string userId, DateTime dateTime);
        Task<ActionResult<bool>> Update(Installment installment);
        Installment Find(int id);
        Task<InstallmentVM> FindVM(int id);
        Task<ActionResult<bool>> SwitchInstallmentState(int id);
    }

    public class InstallmentServices : IInstallmentServices
    {
        private readonly AppDbContext context;

        public InstallmentServices(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetAllUnreceivedVM(string userId)
        {
            return await context.Installments
                .Where(x => x.ReceivedDate == null && x.GetBill.GetClient.UserId == userId)
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .OrderBy(x => x.BillId).ThenBy(x => x.DueDate)
                .Select(x => new InstallmentMobileVM
                {
                    id = x.Id,
                    client = x.GetBill.GetClient.Name,
                    amountValue = x.AmountValue,
                    dueDate = x.DueDate,
                    receivedDate = x.ReceivedDate
                }).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetAllTodayVM(string userId)
        {
            return await context.Installments
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .Where(x => x.GetBill.GetClient.UserId == userId && x.DueDate.Date == DateTime.UtcNow.Date)
                .OrderBy(x => x.BillId).ThenBy(x => x.DueDate)
                .Select(x => new InstallmentMobileVM
                {
                    id = x.Id,
                    client = x.GetBill.GetClient.Name,
                    amountValue = x.AmountValue,
                    dueDate = x.DueDate,
                    receivedDate = x.ReceivedDate
                }).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetReceivedTodayVM(string userId)
        {
            return await context.Installments
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .Where(x => x.GetBill.GetClient.UserId == userId && x.ReceivedDate.Value.Date == DateTime.UtcNow.Date)
                .OrderBy(x => x.BillId).ThenBy(x => x.DueDate)
                .Select(x => new InstallmentMobileVM
                {
                    id = x.Id,
                    client = x.GetBill.GetClient.Name,
                    amountValue = x.AmountValue,
                    dueDate = x.DueDate,
                    receivedDate = x.ReceivedDate
                }).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetUnreceivedTodayVM(string userId)
        {
            return await context.Installments
               .Include(m => m.GetBill)
               .ThenInclude(m => m.GetClient)
               .Where(x => x.GetBill.GetClient.UserId == userId && x.DueDate.Date == DateTime.UtcNow.Date && x.ReceivedDate == null)
               .OrderBy(x => x.BillId).ThenBy(x => x.DueDate)
               .Select(x => new InstallmentMobileVM
               {
                   id = x.Id,
                   client = x.GetBill.GetClient.Name,
                   amountValue = x.AmountValue,
                   dueDate = x.DueDate,
                   receivedDate = x.ReceivedDate
               }).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetByDayVM(string userId, DateTime dateTime)
        {
            return await context.Installments
               .Where(x => x.GetBill.GetClient.UserId == userId && x.DueDate.Date == dateTime.Date)
               .Include(m => m.GetBill)
               .ThenInclude(m => m.GetClient)
               .OrderBy(x => x.BillId).ThenBy(x => x.DueDate)
               .Select(x => new InstallmentMobileVM
               {
                   id = x.Id,
                   client = x.GetBill.GetClient.Name,
                   amountValue = x.AmountValue,
                   dueDate = x.DueDate,
                   receivedDate = x.ReceivedDate
               }).ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<InstallmentMobileVM>>> GetBillInstallmentsVM(int billId)
        {
            return await context.Installments
               .Where(x => x.BillId == billId)
               .Include(m => m.GetBill)
               .ThenInclude(m => m.GetClient)
               .OrderBy(x => x.BillId).ThenBy(x => x.DueDate)
               .Select(x => new InstallmentMobileVM
               {
                   id = x.Id,
                   client = x.GetBill.GetClient.Name,
                   amountValue = x.AmountValue,
                   dueDate = x.DueDate,
                   receivedDate = x.ReceivedDate
               }).ToListAsync();
        }

        public async Task<ActionResult<bool>> Update(Installment installment)
        {
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

        public async Task<InstallmentVM> FindVM(int id)
        {
            return await context.Installments
                .Include(m => m.GetBill)
                .ThenInclude(m => m.GetClient)
                .Select(x => new InstallmentVM
                {
                    id = x.Id,
                    billId = x.BillId,
                    clientName = x.GetBill.GetClient.Name,
                    amountValue = x.AmountValue,
                    dueDate = x.DueDate,
                    receivedDate = x.ReceivedDate,
                    billDescription = x.GetBill.Description,
                    delayFine = x.GetBill.DelayFine,

                    delayFineType = x.GetBill.DelayFineType,
                    installmentType = x.GetBill.InstallmentType
                }).FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<ActionResult<bool>> SwitchInstallmentState(int id)
        {
            Installment installment = await context.Installments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (installment.ReceivedDate == null)
                installment.ReceivedDate = DateTime.UtcNow;
            else
                installment.ReceivedDate = null;

            context.Update(installment);
            await context.SaveChangesAsync();
            return true;
        }

    }
}
