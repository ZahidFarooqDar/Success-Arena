using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Razorpay.Api;
using SuccessArenaBAL.ExceptionHandler;
using SuccessArenaBAL.Foundation.Base;
using SuccessArenaConfig.Configuration;
using SuccessArenaDAL.Context;
using SuccessArenaDomainModels.Enums;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;
using SuccessArenaServiceModels.Foundation.Base.Interfaces;
using SuccessArenaServiceModels.v1.Payment;
using System.Security.Cryptography;
using System.Text;

namespace SuccessArenaBAL.v1
{
    public class PaymentProcess : SuccessArenaBalBase
    {
        private readonly ILoginUserDetail _loginUserDetail;
        private readonly APIConfiguration _apiConfiguration;
        private readonly UserTestPackageProcess _userTestPackageProcess;

        public PaymentProcess(
            IMapper mapper,
            ApiDbContext context,
            ILoginUserDetail loginUserDetail,
            APIConfiguration apiConfiguration,
            UserTestPackageProcess userTestPackageProcess)
            : base(mapper, context)
        {
            _loginUserDetail = loginUserDetail;
            _apiConfiguration = apiConfiguration;
            _userTestPackageProcess = userTestPackageProcess;
        }
        #region Create
        public async Task<RazorPayOrderSM> CreatePaymentOrder(
    int packageId)
        {
            int userId =
                Convert.ToInt32(
                    _loginUserDetail.LoginId);

            var package =
                await _apiDbContext.TestPackages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == packageId &&
                        x.IsActive);

            if (package == null)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    $"Package not found. Id:{packageId}",
                    "Package details not found.");
            }

            RazorpayClient client =
                new RazorpayClient(
                    _apiConfiguration.RazorPaySettings.Key,
                    _apiConfiguration.RazorPaySettings.Secret);
            var receiptId = $"RPT:{packageId}-{Guid.NewGuid():N}";
            Dictionary<string, object> options =
                new()
                {
            { "amount", Convert.ToInt32(package.Price * 100) },
            { "currency", "INR" },
            { "receipt", $"{receiptId}" }
                };

            Order order =
                client.Order.Create(options);

            string razorPayOrderId =
                order["id"].ToString();

            var subscription =
                await _userTestPackageProcess
                    .CreatePendingSubscription(
                        userId,
                        packageId,
                        razorPayOrderId,
                        receiptId);

            return new RazorPayOrderSM
            {
                OrderId = razorPayOrderId,
                Amount = package.Price,
                Currency = "INR",
                UserPackageId = subscription.Id
            };
        }

        #endregion Create

        #region Verify

        public async Task<BoolResponseRoot> VerifyPayment(
    VerifyPaymentSM sm)
        {
            bool isValid =
                ValidateRazorPaySignature(
                    sm.RazorPayOrderId,
                    sm.RazorPayPaymentId,
                    sm.RazorPaySignature);
            var secret = _apiConfiguration.RazorPaySettings.Secret;
            if (!isValid)
            {
                await _userTestPackageProcess
                    .MarkPaymentFailed(
                        sm.RazorPayOrderId);

                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invalid payment signature.",
                    "Payment verification failed.");
            }

            await _userTestPackageProcess
                .ConfirmPayment(
                    sm.RazorPayOrderId,
                    sm.RazorPayPaymentId,
                    sm.RazorPaySignature,
                    secret);

            return new BoolResponseRoot(
                true,
                "Payment verified successfully.");
        }

        #endregion Verify

        #region Refund

        public async Task<BoolResponseRoot> RefundPayment(
    int invoiceId)
        {
            var invoice =
                await _apiDbContext.Invoices
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

            if (invoice.PaymentStatus !=
                PaymentStatusDM.Success)
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Refund not allowed.",
                    "Only successful payments can be refunded.");
            }

            RazorpayClient client =
                new RazorpayClient(
                    _apiConfiguration.RazorPaySettings.Key,
                    _apiConfiguration.RazorPaySettings.Secret);

            Payment payment =
                client.Payment.Fetch(
                    invoice.RazorPayPaymentId);

            payment.Refund();

            invoice.PaymentStatus =
                PaymentStatusDM.Refunded;

            invoice.LastModifiedBy =
                _loginUserDetail.LoginId;

            invoice.LastModifiedOnUTC =
                DateTime.UtcNow;

            invoice.UserTestPackage.PaymentStatus =
                PaymentStatusDM.Refunded;

            invoice.UserTestPackage.IsActive =
                false;

            await _apiDbContext.SaveChangesAsync();

            return new BoolResponseRoot(
                true,
                "Payment refunded successfully.");
        }

        #endregion Refund
        #region Validate Signature

        private bool ValidateRazorPaySignature(
    string orderId,
    string paymentId,
    string razorPaySignature)
        {
            string payload =
                $"{orderId}|{paymentId}";

            using var hmac =
                new HMACSHA256(
                    Encoding.UTF8.GetBytes(
                        _apiConfiguration
                            .RazorPaySettings
                            .Secret));

            byte[] hash =
                hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(payload));

            string generatedSignature =
                BitConverter
                    .ToString(hash)
                    .Replace("-", "")
                    .ToLower();

            return generatedSignature ==
                   razorPaySignature.ToLower();
        }

        #endregion Validate Signature
        #region Web Hook

        public async Task ProcessWebhook(
    string payload,
    string signature)
        {
            var secret = _apiConfiguration.RazorPaySettings.Secret;
            using var hmac =
                new HMACSHA256(
                    Encoding.UTF8.GetBytes(
                        _apiConfiguration
                            .RazorPaySettings
                            .Secret));

            byte[] hash =
                hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(payload));

            string generatedSignature =
                Convert.ToHexString(hash)
                    .ToLower();

            if (generatedSignature !=
                signature.ToLower())
            {
                throw new SuccessArenaException(
                    ApiErrorTypeSM.InvalidInputData_NoLog,
                    "Invalid webhook signature.",
                    "Webhook verification failed.");
            }

            dynamic webhook =
                JsonConvert.DeserializeObject(payload);

            string eventName =
                webhook.@event;

            switch (eventName)
            {
                case "payment.captured":

                    string orderId =
                        webhook.payload.payment.entity.order_id;

                    string paymentId =
                        webhook.payload.payment.entity.id;

                    string razorSignature =
                        signature;

                    await _userTestPackageProcess
                        .ConfirmPayment(
                            orderId,
                            paymentId,
                            razorSignature,
                            secret);

                    break;

                case "payment.failed":

                    string failedOrderId =
                        webhook.payload.payment.entity.order_id;

                    await _userTestPackageProcess
                        .MarkPaymentFailed(
                            failedOrderId);

                    break;
            }
        }

        #endregion Web Hook
    }
}
