using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SuccessArenaBAL.ExceptionHandler;
using SuccessArenaBAL.Foundation.Base;
using SuccessArenaDAL.Context;
using SuccessArenaDomainModels.Enums;
using SuccessArenaDomainModels.v1;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;
using SuccessArenaServiceModels.Foundation.Base.Interfaces;
using SuccessArenaServiceModels.v1;

namespace SuccessArenaBAL.v1
{
    public class InvoiceProcess : SuccessArenaBalOdataBase<InvoiceSM>
    {
        protected readonly ILoginUserDetail _loginUserDetail;

        public InvoiceProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #region OData

        public override async Task<IQueryable<InvoiceSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.Invoices;

            IQueryable<InvoiceSM> retSM =
                await MapEntityAsToQuerable<InvoiceDM, InvoiceSM>(_mapper, entitySet);

            return retSM;
        }

        #endregion

        #region Get

        #region Get By Id
        
        public async Task<InvoiceSM> GetInvoiceById(
    int id)
        {
            var dm = await _apiDbContext.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invoice not found. Id:{id}",
                    "Invoice details not found.");
            }

            return _mapper.Map<InvoiceSM>(dm);
        }

        public async Task<InvoiceSM> GetInvoiceByInvoiceNumber(
            string invoiceNumber)
        {
            var dm = await _apiDbContext.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.InvoiceNumber == invoiceNumber);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invoice not found. Invoice Number:{invoiceNumber}",
                    "Invoice details not found.");
            }

            return _mapper.Map<InvoiceSM>(dm);
        }

        public async Task<InvoiceSM> GetInvoiceByRazorPayOrderId(
            string razorPayOrderId)
        {
            var dm = await _apiDbContext.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.RazorPayOrderId == razorPayOrderId);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invoice not found. RazorPay Order Id:{razorPayOrderId}",
                    "Invoice details not found.");
            }

            return _mapper.Map<InvoiceSM>(dm);
        }

        public async Task<InvoiceSM> GetInvoiceByRazorPayPaymentId(
            string razorPayPaymentId)
        {
            var dm = await _apiDbContext.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.RazorPayPaymentId == razorPayPaymentId);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invoice not found. RazorPay Payment Id:{razorPayPaymentId}",
                    "Invoice details not found.");
            }

            return _mapper.Map<InvoiceSM>(dm);
        }
        #endregion Get By id

        #region Get All

        public async Task<List<InvoiceSM>> GetAllInvoices(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Invoices
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<InvoiceSM>>(dms);
        }

        public async Task<List<InvoiceSM>> GetInvoicesByUser(
            int userId,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x => x.ClientUserId == userId)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<InvoiceSM>>(dms);
        }

        public async Task<List<InvoiceSM>> GetInvoicesByPaymentStatus(
            PaymentStatusDM paymentStatus,
            int skip,
            int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus == paymentStatus)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<InvoiceSM>>(dms);
        }

        #endregion Get All

        #region Pending

        public async Task<List<InvoiceSM>> GetPendingInvoices(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Pending)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<InvoiceSM>>(dms);
        }

        #endregion Pending

        #region Get Success

        public async Task<List<InvoiceSM>> GetSuccessfulInvoices(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Success)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<InvoiceSM>>(dms);
        }

        #endregion Get Success

        #region Failed

        public async Task<List<InvoiceSM>> GetFailedInvoices(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Failed)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<InvoiceSM>>(dms);
        }

        #endregion Failed

        #region Refunded

        public async Task<List<InvoiceSM>> GetRefundedInvoices(
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Refunded)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<InvoiceSM>>(dms);
        }

        #endregion Refunded

        #endregion Get

        #region Count

        public async Task<IntResponseRoot> GetInvoiceCount()
        {
            var count = await _apiDbContext.Invoices
                .AsNoTracking()
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total invoices.");
        }

        public async Task<IntResponseRoot> GetInvoiceCountByUser(
            int userId)
        {
            var count = await _apiDbContext.Invoices
                .AsNoTracking()
                .CountAsync(x =>
                    x.ClientUserId == userId);

            return new IntResponseRoot(
                count,
                "Total user invoices.");
        }

        public async Task<IntResponseRoot> GetPendingInvoiceCount()
        {
            var count = await _apiDbContext.Invoices
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Pending);

            return new IntResponseRoot(
                count,
                "Pending invoices.");
        }

        public async Task<IntResponseRoot> GetSuccessfulInvoiceCount()
        {
            var count = await _apiDbContext.Invoices
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Success);

            return new IntResponseRoot(
                count,
                "Successful invoices.");
        }

        public async Task<IntResponseRoot> GetFailedInvoiceCount()
        {
            var count = await _apiDbContext.Invoices
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Failed);

            return new IntResponseRoot(
                count,
                "Failed invoices.");
        }

        public async Task<IntResponseRoot> GetRefundedInvoiceCount()
        {
            var count = await _apiDbContext.Invoices
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Refunded);

            return new IntResponseRoot(
                count,
                "Refunded invoices.");
        }


        #endregion Count

        #region Revenue

        public async Task<DecimalResponseRoot> GetTotalRevenue()
        {
            
            var revenue = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus ==
                    PaymentStatusDM.Success)
                .SumAsync(x => x.TotalAmount);

            return new DecimalResponseRoot(
                revenue,
                "Total revenue.");
        }

        public async Task<DecimalResponseRoot> GetRevenueByUser(
            int userId)
        {
            var revenue = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.ClientUserId == userId
                    && x.PaymentStatus ==
                    PaymentStatusDM.Success)
                .SumAsync(x => x.TotalAmount);

            return new DecimalResponseRoot(
                revenue,
                "User revenue.");
        }

        #endregion Revenue

        #region Status Update

        public async Task<BoolResponseRoot> MarkInvoiceFailed(
    int invoiceId,
    string? paymentReference = null)
        {
            var invoice = await _apiDbContext.Invoices
                .Include(x => x.UserTestPackage)
                .FirstOrDefaultAsync(x =>
                    x.Id == invoiceId);

            if (invoice == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invoice not found. Id:{invoiceId}",
                    "Invoice details not found.");
            }

            invoice.PaymentStatus =
                PaymentStatusDM.Failed;

            invoice.PaymentReference =
                paymentReference;

            invoice.LastModifiedBy =
                _loginUserDetail.LoginId;

            invoice.LastModifiedOnUTC =
                DateTime.UtcNow;

            if (invoice.UserTestPackage != null)
            {
                invoice.UserTestPackage.PaymentStatus =
                    PaymentStatusDM.Failed;

                invoice.UserTestPackage.IsActive =
                    false;
            }

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Invoice marked failed.");
        }

        public async Task<BoolResponseRoot> MarkInvoiceRefunded(
            int invoiceId,
            string? paymentReference = null)
        {
            var invoice = await _apiDbContext.Invoices
                .Include(x => x.UserTestPackage)
                .FirstOrDefaultAsync(x =>
                    x.Id == invoiceId);

            if (invoice == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Invoice not found. Id:{invoiceId}",
                    "Invoice details not found.");
            }

            invoice.PaymentStatus =
                PaymentStatusDM.Refunded;

            invoice.PaymentReference =
                paymentReference;

            invoice.LastModifiedBy =
                _loginUserDetail.LoginId;

            invoice.LastModifiedOnUTC =
                DateTime.UtcNow;

            if (invoice.UserTestPackage != null)
            {
                invoice.UserTestPackage.PaymentStatus =
                    PaymentStatusDM.Refunded;

                invoice.UserTestPackage.IsActive =
                    false;
            }

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Invoice refunded successfully.");
        }

        #endregion Status Update
    }
}