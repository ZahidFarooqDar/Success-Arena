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
using System.Security.Cryptography;
using System.Text;

namespace SuccessArenaBAL.v1
{
    public class UserTestPackageProcess : SuccessArenaBalOdataBase<UserTestPackageSM>
    {
        protected readonly ILoginUserDetail _loginUserDetail;

        public UserTestPackageProcess(
            IMapper mapper,
            ILoginUserDetail loginUserDetail,
            ApiDbContext apiDbContext)
            : base(mapper, apiDbContext)
        {
            _loginUserDetail = loginUserDetail;
        }

        #region OData

        public override async Task<IQueryable<UserTestPackageSM>> GetServiceModelEntitiesForOdata()
        {
            var entitySet = _apiDbContext.UserTestPackages;

            IQueryable<UserTestPackageSM> retSM =
                await MapEntityAsToQuerable<UserTestPackageDM, UserTestPackageSM>(_mapper, entitySet);

            return retSM;
        }

        #endregion

        #region Get

        public async Task<UserTestPackageSM> GetUserPackageById(
    int id)
        {
            var dm = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"User Package not found. Id: {id}",
                    "Package details not found.");
            }

            return _mapper.Map<UserTestPackageSM>(dm);
        }

        public async Task<List<UserTestPackageSM>> GetPackagesByUser(
    int userId,
    int skip,
    int top)
        {
            skip = Math.Max(0, skip);
            top = top <= 0 ? 10 : top;

            var dms = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .Where(x => x.ClientUserId == userId)
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(top)
                .ToListAsync();

            return _mapper.Map<List<UserTestPackageSM>>(dms);
        }

        public async Task<List<UserTestPackageSM>> GetActivePackagesByUser(
    int userId)
        {
            var dms = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .Where(x =>
                    x.ClientUserId == userId
                    && x.IsActive
                    && x.PaymentStatus == PaymentStatusDM.Success
                    && x.AttemptsUsed < x.AllowedAttempts
                    && (!x.ExpiryDate.HasValue ||
                        x.ExpiryDate >= DateTime.UtcNow))
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return _mapper.Map<List<UserTestPackageSM>>(dms);
        }

        public async Task<UserTestPackageSM> GetCurrentSubscription(
    int userId,
    int packageId)
        {
            var dm = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .Where(x =>
                    x.ClientUserId == userId
                    && x.TestPackageId == packageId
                    && x.IsActive
                    && x.PaymentStatus == PaymentStatusDM.Success
                    && x.AttemptsUsed < x.AllowedAttempts
                    && (!x.ExpiryDate.HasValue ||
                        x.ExpiryDate >= DateTime.UtcNow))
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "No active subscription found.",
                    "Please purchase package.");
            }

            return _mapper.Map<UserTestPackageSM>(dm);
        }

        public async Task<BoolResponseRoot> HasUserAccessToPackage(
    int userId,
    int packageId)
        {
            bool hasAccess = await _apiDbContext.UserTestPackages
                .AnyAsync(x =>
                    x.ClientUserId == userId
                    && x.TestPackageId == packageId
                    && x.IsActive
                    && x.PaymentStatus == PaymentStatusDM.Success
                    && x.AttemptsUsed < x.AllowedAttempts
                    && (!x.ExpiryDate.HasValue ||
                        x.ExpiryDate >= DateTime.UtcNow));

            return new BoolResponseRoot(
                hasAccess,
                hasAccess
                    ? "User has access."
                    : "User does not have access.");
        }

        public async Task<IntResponseRoot> GetRemainingAttempts(
    int userPackageId)
        {
            var dm = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userPackageId);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"User Package not found. Id:{userPackageId}",
                    "Package details not found.");
            }

            return new IntResponseRoot(
                dm.AllowedAttempts - dm.AttemptsUsed,
                "Remaining attempts.");
        }

        public async Task<List<UserTestPackageSM>>
GetExpiredPackagesByUser(
    int userId)
        {
            var dms = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .Where(x =>
                    x.ClientUserId == userId &&
                    x.ExpiryDate.HasValue &&
                    x.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            return _mapper.Map<List<UserTestPackageSM>>(dms);
        }

        public async Task<List<UserTestPackageSM>>
GetConsumedPackagesByUser(
    int userId)
        {
            var dms = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .Where(x =>
                    x.ClientUserId == userId &&
                    x.AttemptsUsed >= x.AllowedAttempts)
                .ToListAsync();

            return _mapper.Map<List<UserTestPackageSM>>(dms);
        }

        public async Task<List<UserTestPackageSM>>
GetAllUserPackages(
    int skip,
    int top)
        {
            return _mapper.Map<List<UserTestPackageSM>>(
                await _apiDbContext.UserTestPackages
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Skip(skip)
                    .Take(top)
                    .ToListAsync());
        }

        public async Task<List<UserTestPackageSM>>
GetAllPendingUserPackages(
    int skip,
    int top)
        {
            return _mapper.Map<List<UserTestPackageSM>>(
                await _apiDbContext.UserTestPackages
                    .AsNoTracking()
                    .Where(x =>
                        x.PaymentStatus ==
                        PaymentStatusDM.Pending)
                    .OrderByDescending(x => x.Id)
                    .Skip(skip)
                    .Take(top)
                    .ToListAsync());
        }

        public async Task<List<UserTestPackageSM>>
GetUsersByPackage(
    int packageId,
    int skip,
    int top)
        {
            return _mapper.Map<List<UserTestPackageSM>>(
                await _apiDbContext.UserTestPackages
                    .AsNoTracking()
                    .Where(x =>
                        x.TestPackageId == packageId)
                    .OrderByDescending(x => x.Id)
                    .Skip(skip)
                    .Take(top)
                    .ToListAsync());
        }

        public async Task<List<UserTestPackageSM>>
GetPackagePurchaseHistory(
    int userId,
    int packageId)
        {
            return _mapper.Map<List<UserTestPackageSM>>(
                await _apiDbContext.UserTestPackages
                    .AsNoTracking()
                    .Where(x =>
                        x.ClientUserId == userId &&
                        x.TestPackageId == packageId)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync());
        }

        #endregion Get

        #region Subscription Methods

        public async Task<BoolResponseRoot>
CancelPendingSubscription(
    int userPackageId)
        {
            var dm =
                await _apiDbContext.UserTestPackages
                    .FirstOrDefaultAsync(x =>
                        x.Id == userPackageId);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Subscription not found.",
                    "Subscription not found.");
            }

            dm.IsActive = false;
            dm.PaymentStatus =
                PaymentStatusDM.Cancelled;

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Subscription cancelled.");
        }

        public async Task<BoolResponseRoot> MarkPaymentFailed(
    string razorPayOrderId)
        {
            var invoice =
                await _apiDbContext.Invoices
                    .Include(x => x.UserTestPackage)
                    .FirstOrDefaultAsync(x =>
                        x.RazorPayOrderId ==
                        razorPayOrderId);

            if (invoice == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invoice not found.",
                    "Invoice details not found.");
            }

            invoice.PaymentStatus =
                PaymentStatusDM.Failed;

            invoice.UserTestPackage.PaymentStatus =
                PaymentStatusDM.Failed;

            invoice.UserTestPackage.IsActive =
                false;

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Payment marked as failed.");
        }

        public async Task<BoolResponseRoot>
MarkPaymentRefunded(
    int userPackageId)
        {
            var dm =
                await _apiDbContext.UserTestPackages
                    .FirstOrDefaultAsync(x =>
                        x.Id == userPackageId);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Package not found.",
                    "Package not found.");
            }

            dm.PaymentStatus =
                PaymentStatusDM.Refunded;

            dm.IsActive = false;

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Payment refunded.");
        }
        #region Validate

        private bool ValidateRazorPaySignature(
    string razorPayOrderId,
    string razorPayPaymentId,
    string razorPaySignature,
    string secret)
        {
            string payload =
                $"{razorPayOrderId}|{razorPayPaymentId}";

            using var hmac =
                new HMACSHA256(
                    Encoding.UTF8.GetBytes(secret));

            var hash =
                hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(payload));

            string generatedSignature =
                Convert.ToHexString(hash)
                    .ToLower();

            return generatedSignature ==
                   razorPaySignature;
        }

        #endregion Validate

        #endregion Subscription Methods

        #region Count

        public async Task<IntResponseRoot>
GetPackageCountByUser(
    int userId)
        {
            var count =
                await _apiDbContext.UserTestPackages
                    .CountAsync(x =>
                        x.ClientUserId == userId);

            return new IntResponseRoot(
                count,
                "Total purchased packages.");
        }

        public async Task<IntResponseRoot>
GetActivePackageCountByUser(
    int userId)
        {
            var count =
                await _apiDbContext.UserTestPackages
                    .CountAsync(x =>
                        x.ClientUserId == userId
                        && x.IsActive
                        && x.PaymentStatus ==
                           PaymentStatusDM.Success);

            return new IntResponseRoot(
                count,
                "Total active packages.");
        }

        public async Task<IntResponseRoot>
GetPendingPackageCountByUser(
    int userId)
        {
            var count =
                await _apiDbContext.UserTestPackages
                    .CountAsync(x =>
                        x.ClientUserId == userId
                        && x.PaymentStatus ==
                           PaymentStatusDM.Pending);

            return new IntResponseRoot(
                count,
                "Total pending packages.");
        }

        public async Task<IntResponseRoot>
GetFailedPackageCountByUser(
    int userId)
        {
            var count =
                await _apiDbContext.UserTestPackages
                    .CountAsync(x =>
                        x.ClientUserId == userId
                        && x.PaymentStatus ==
                           PaymentStatusDM.Failed);

            return new IntResponseRoot(
                count,
                "Total failed packages.");
        }
        public async Task<IntResponseRoot>
GetConsumedPackageCountByUser(
    int userId)
        {
            var count =
                await _apiDbContext.UserTestPackages
                    .CountAsync(x =>
                        x.ClientUserId == userId
                        && x.AttemptsUsed >=
                           x.AllowedAttempts);

            return new IntResponseRoot(
                count,
                "Total consumed packages.");
        }

        public async Task<IntResponseRoot>
GetExpiredPackageCountByUser(
    int userId)
        {
            var count =
                await _apiDbContext.UserTestPackages
                    .CountAsync(x =>
                        x.ClientUserId == userId
                        && x.ExpiryDate.HasValue
                        && x.ExpiryDate <
                           DateTime.UtcNow);

            return new IntResponseRoot(
                count,
                "Total expired packages.");
        }

        #region Admin Counts

        public async Task<IntResponseRoot>
GetTotalUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync();

            return new IntResponseRoot(
                count,
                "Total user packages.");
        }

        public async Task<IntResponseRoot>
GetTotalActiveUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.IsActive &&
                    x.PaymentStatus == PaymentStatusDM.Success &&
                    x.AttemptsUsed < x.AllowedAttempts &&
                    (!x.ExpiryDate.HasValue ||
                     x.ExpiryDate >= DateTime.UtcNow));

            return new IntResponseRoot(
                count,
                "Total active user packages.");
        }

        public async Task<IntResponseRoot>
GetTotalPendingUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus == PaymentStatusDM.Pending);

            return new IntResponseRoot(
                count,
                "Total pending user packages.");
        }

        public async Task<IntResponseRoot>
GetTotalFailedUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus == PaymentStatusDM.Failed);

            return new IntResponseRoot(
                count,
                "Total failed user packages.");
        }

        public async Task<IntResponseRoot>
GetTotalRefundedUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus == PaymentStatusDM.Refunded);

            return new IntResponseRoot(
                count,
                "Total refunded user packages.");
        }

        public async Task<IntResponseRoot>
GetTotalCancelledUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus == PaymentStatusDM.Cancelled);

            return new IntResponseRoot(
                count,
                "Total cancelled user packages.");
        }

        public async Task<IntResponseRoot>
GetTotalConsumedUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.AttemptsUsed >= x.AllowedAttempts);

            return new IntResponseRoot(
                count,
                "Total consumed user packages.");
        }

        public async Task<IntResponseRoot>
GetTotalExpiredUserPackageCount()
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.ExpiryDate.HasValue &&
                    x.ExpiryDate < DateTime.UtcNow);

            return new IntResponseRoot(
                count,
                "Total expired user packages.");
        }
        public async Task<IntResponseRoot>
GetPackagePurchaseCount(
    int packageId)
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.TestPackageId == packageId);

            return new IntResponseRoot(
                count,
                "Total package purchases.");
        }

        public async Task<IntResponseRoot>
GetActivePackagePurchaseCount(
    int packageId)
        {
            var count = await _apiDbContext.UserTestPackages
                .AsNoTracking()
                .CountAsync(x =>
                    x.TestPackageId == packageId &&
                    x.IsActive &&
                    x.PaymentStatus == PaymentStatusDM.Success &&
                    x.AttemptsUsed < x.AllowedAttempts);

            return new IntResponseRoot(
                count,
                "Total active package purchases.");
        }

        public async Task<DecimalResponseRoot> GetPackageRevenue(
    int packageId)
        {
            var revenue = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.UserTestPackage.TestPackageId == packageId &&
                    x.PaymentStatus == PaymentStatusDM.Success)
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

            return new DecimalResponseRoot(
                revenue,
                "Package revenue.");
        }

        public async Task<DecimalResponseRoot> GetTotalRevenue()
        {
            var revenue = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus == PaymentStatusDM.Success)
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

            return new DecimalResponseRoot(
                revenue,
                "Total revenue.");
        }
        public async Task<DecimalResponseRoot>
GetRevenueByUser(
    int userId)
        {
            var revenue = await _apiDbContext.Invoices
                .AsNoTracking()
                .Where(x =>
                    x.ClientUserId == userId &&
                    x.PaymentStatus == PaymentStatusDM.Success)
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

            return new DecimalResponseRoot(
                revenue,
                "User revenue.");
        }
        #endregion Admin Counts

        #endregion Count

        #region Purchase Methods

        public async Task<UserTestPackageSM> CreateSubscriptionAfterPayment(
    int userId,
    int packageId,
    string razorPayOrderId,
    string razorPayPaymentId,
    string razorPaySignature)
        {
            var package = await _apiDbContext.TestPackages
                .FirstOrDefaultAsync(x => x.Id == packageId);

            if (package == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package not found. Id:{packageId}",
                    "Package details not found.");
            }

            using var transaction =
                await _apiDbContext.Database.BeginTransactionAsync();

            try
            {
                var userPackage = new UserTestPackageDM
                {
                    ClientUserId = userId,
                    TestPackageId = packageId,
                    AllowedAttempts = package.AllowedAttempts,
                    AttemptsUsed = 0,
                    PurchasedOn = DateTime.UtcNow,
                    IsActive = true,
                    PaymentStatus = PaymentStatusDM.Success,
                    CreatedBy = _loginUserDetail.LoginId,
                    CreatedOnUTC = DateTime.UtcNow
                };

                await _apiDbContext.UserTestPackages
                    .AddAsync(userPackage);

                await _apiDbContext.SaveChangesAsync();

                var invoice = new InvoiceDM
                {
                    ClientUserId = userId,
                    UserTestPackageId = userPackage.Id,
                    InvoiceNumber = GenerateInvoiceNumber(),
                    PaymentStatus = PaymentStatusDM.Success,
                    PaymentDateUtc = DateTime.UtcNow,
                    RazorPayOrderId = razorPayOrderId,
                    RazorPayPaymentId = razorPayPaymentId,
                    RazorPaySignature = razorPaySignature,
                    TotalAmount = package.Price,
                    PaymentGateway = "RazorPay",
                    TransactionId = razorPayPaymentId,
                    CreatedBy = _loginUserDetail.LoginId,
                    CreatedOnUTC = DateTime.UtcNow
                };

                await _apiDbContext.Invoices
                    .AddAsync(invoice);

                await _apiDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetUserPackageById(userPackage.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<UserTestPackageSM> CreatePendingSubscription(
    int userId,
    int packageId,
    string razorPayOrderId,
    string recieptId)
        {
            var package = await _apiDbContext.TestPackages
                .FirstOrDefaultAsync(x => x.Id == packageId);

            if (package == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package not found. Id:{packageId}",
                    "Package details not found.");
            }

            using var transaction =
                await _apiDbContext.Database.BeginTransactionAsync();

            try
            {
                var userPackage = new UserTestPackageDM
                {
                    ClientUserId = userId,
                    TestPackageId = packageId,
                    AllowedAttempts = package.AllowedAttempts,
                    AttemptsUsed = 0,
                    PurchasedOn = DateTime.UtcNow,
                    IsActive = false,
                    RecieptId=  recieptId,
                    PaymentStatus = PaymentStatusDM.Pending,
                    CreatedBy = _loginUserDetail.LoginId,
                    CreatedOnUTC = DateTime.UtcNow
                };

                await _apiDbContext.UserTestPackages
                    .AddAsync(userPackage);

                await _apiDbContext.SaveChangesAsync();

                var invoice = new InvoiceDM
                {
                    ClientUserId = userId,
                    UserTestPackageId = userPackage.Id,
                    InvoiceNumber = GenerateInvoiceNumber(),
                    PaymentStatus = PaymentStatusDM.Pending,
                    PaymentDateUtc = DateTime.UtcNow,
                    RazorPayOrderId = razorPayOrderId,
                    RazorPayPaymentId = string.Empty,
                    RazorPaySignature = string.Empty,
                    TotalAmount = package.Price,
                    PaymentGateway = "RazorPay",
                    CreatedBy = _loginUserDetail.LoginId,
                    CreatedOnUTC = DateTime.UtcNow
                };

                await _apiDbContext.Invoices
                    .AddAsync(invoice);

                await _apiDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetUserPackageById(userPackage.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public async Task<BoolResponseRoot>
ConfirmPayment(
    string razorPayOrderId,
    string razorPayPaymentId,
    string razorPaySignature,
    string razorPaySecret)
        {
            using var transaction =
                await _apiDbContext.Database
                    .BeginTransactionAsync();

            try
            {
                var invoice = await _apiDbContext.Invoices
                    .Include(x => x.UserTestPackage)
                    .FirstOrDefaultAsync(x =>
                        x.RazorPayOrderId ==
                        razorPayOrderId);

                if (invoice == null)
                {
                    throw new SuccessArenaException(
                        ApiErrorTypeSM.InvalidInputData_NoLog,
                        "Invoice not found.",
                        "Invoice details not found.");
                }

                if (invoice.PaymentStatus ==
                    PaymentStatusDM.Success)
                {
                    return new BoolResponseRoot(
                        true,
                        "Payment already processed.");
                }

                bool isValid =
                    ValidateRazorPaySignature(
                        razorPayOrderId,
                        razorPayPaymentId,
                        razorPaySignature,
                        razorPaySecret);

                if (!isValid)
                {
                    throw new SuccessArenaException(
                        ApiErrorTypeSM.InvalidInputData_NoLog,
                        "Invalid payment signature.",
                        "Payment verification failed.");
                }

                invoice.PaymentStatus =
                    PaymentStatusDM.Success;

                invoice.PaymentDateUtc =
                    DateTime.UtcNow;

                invoice.RazorPayPaymentId =
                    razorPayPaymentId;

                invoice.RazorPaySignature =
                    razorPaySignature;

                invoice.TransactionId =
                    razorPayPaymentId;

                invoice.LastModifiedBy =
                    _loginUserDetail.LoginId;

                invoice.LastModifiedOnUTC =
                    DateTime.UtcNow;

                invoice.UserTestPackage.PaymentStatus =
                    PaymentStatusDM.Success;

                invoice.UserTestPackage.IsActive =
                    true;

                invoice.UserTestPackage.LastModifiedBy =
                    _loginUserDetail.LoginId;

                invoice.UserTestPackage.LastModifiedOnUTC =
                    DateTime.UtcNow;

                await _apiDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new BoolResponseRoot(
                    true,
                    "Payment confirmed successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion Purchase Methods

        #region Consume Attempt

        public async Task<BoolResponseRoot> ConsumeAttempt(
    int userPackageId)
        {
            var dm = await _apiDbContext.UserTestPackages
                .FirstOrDefaultAsync(x => x.Id == userPackageId);

            if (dm == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package not found. Id:{userPackageId}",
                    "Package details not found.");
            }

            if (dm.AttemptsUsed >= dm.AllowedAttempts)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Attempts exhausted.",
                    "Please purchase package again.");
            }

            dm.AttemptsUsed++;

            if (dm.AttemptsUsed >= dm.AllowedAttempts)
            {
                dm.IsActive = false;
            }

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Attempt consumed successfully.");
        }

        #endregion Consume Attempt
    }
}