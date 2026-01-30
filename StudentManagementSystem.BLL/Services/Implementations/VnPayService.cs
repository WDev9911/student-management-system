using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Helpers;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;

        public VnPayService(
            IConfiguration configuration,
            IWalletRepository walletRepo,
            ITransactionRepository transactionRepo)
        {
            _configuration = configuration;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
        }

        // Implementation matches IVnPayService: accepts client IP string
        public string CreatePaymentUrl(VnPayPaymentRequest request, string clientIp)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["VnPay:ReturnUrl"];

            pay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)(request.Amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", string.IsNullOrEmpty(clientIp) ? "127.0.0.1" : clientIp);
            pay.AddRequestData("vnp_Locale", request.Locale);
            pay.AddRequestData("vnp_OrderInfo", $"Student {request.StudentId} - {request.Description}");
            pay.AddRequestData("vnp_OrderType", request.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack!);
            pay.AddRequestData("vnp_TxnRef", tick);
            pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            // Use CreateRequestUrl defined in VnPayLibrary
            var paymentUrl = pay.CreateRequestUrl(_configuration["VnPay:BaseUrl"]!, _configuration["VnPay:HashSecret"]!);

            return paymentUrl;
        }

        // Implementation matches IVnPayService: accepts IDictionary<string,string>
        public VnPayCallbackModel ProcessCallback(IDictionary<string, string> queryParams)
        {
            var pay = new VnPayLibrary();
            var callback = new VnPayCallbackModel();

            foreach (var kv in queryParams)
            {
                var key = kv.Key;
                var value = kv.Value;
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(key, value);
                }
            }

            callback.vnp_TmnCode = pay.GetResponseData("vnp_TmnCode");
            callback.vnp_Amount = pay.GetResponseData("vnp_Amount");
            callback.vnp_BankCode = pay.GetResponseData("vnp_BankCode");
            callback.vnp_BankTranNo = pay.GetResponseData("vnp_BankTranNo");
            callback.vnp_CardType = pay.GetResponseData("vnp_CardType");
            callback.vnp_PayDate = pay.GetResponseData("vnp_PayDate");
            callback.vnp_OrderInfo = pay.GetResponseData("vnp_OrderInfo");
            callback.vnp_TransactionNo = pay.GetResponseData("vnp_TransactionNo");
            callback.vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
            callback.vnp_TransactionStatus = pay.GetResponseData("vnp_TransactionStatus");
            callback.vnp_TxnRef = pay.GetResponseData("vnp_TxnRef");
            callback.vnp_SecureHash = queryParams.TryGetValue("vnp_SecureHash", out var s) ? s : string.Empty;

            return callback;
        }

        public async Task<bool> ProcessPaymentAsync(VnPayCallbackModel callback)
        {
            try
            {
                // Validate signature
                var pay = new VnPayLibrary();
                foreach (var prop in callback.GetType().GetProperties())
                {
                    var value = prop.GetValue(callback)?.ToString();
                    if (!string.IsNullOrEmpty(value) && prop.Name != nameof(callback.vnp_SecureHash))
                    {
                        pay.AddResponseData(prop.Name, value);
                    }
                }

                bool isValidSignature = pay.ValidateSignature(callback.vnp_SecureHash, _configuration["VnPay:HashSecret"]!);
                if (!isValidSignature)
                {
                    return false;
                }

                // Parse data
                long vnpAmount = long.Parse(callback.vnp_Amount) / 100;
                string txnRef = callback.vnp_TxnRef;

                // Extract student ID from order info
                var orderInfo = callback.vnp_OrderInfo ?? string.Empty;
                var parts = orderInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !int.TryParse(parts[1], out int studentId))
                {
                    return false;
                }

                // Get or create wallet
                var wallet = await _walletRepo.GetByStudentIdAsync(studentId);
                if (wallet == null)
                {
                    wallet = new Wallet
                    {
                        StudentId = studentId,
                        Balance = 0,
                        CreatedDate = DateTime.Now,
                        LastUpdated = DateTime.Now
                    };
                    wallet = await _walletRepo.CreateAsync(wallet);
                }

                // Check if transaction already exists
                var existingTxn = await _transactionRepo.GetByTransactionCodeAsync(txnRef);
                if (existingTxn != null)
                {
                    return existingTxn.Status == "Success";
                }

                // Create transaction
                bool isSuccess = callback.vnp_ResponseCode == "00" && callback.vnp_TransactionStatus == "00";

                var transaction = new Transaction
                {
                    WalletId = wallet.WalletId,
                    StudentId = studentId,
                    TransactionCode = txnRef,
                    Type = "Deposit",
                    Amount = vnpAmount,
                    BalanceBefore = wallet.Balance,
                    BalanceAfter = isSuccess ? wallet.Balance + vnpAmount : wallet.Balance,
                    Description = $"N?p ti?n qua VNPay - {orderInfo}",
                    Status = isSuccess ? "Success" : "Failed",
                    PaymentMethod = $"VNPay - {callback.vnp_BankCode}",
                    CreatedDate = DateTime.Now
                };

                await _transactionRepo.CreateAsync(transaction);

                // Update wallet balance if success
                if (isSuccess)
                {
                    wallet.Balance += vnpAmount;
                    wallet.LastUpdated = DateTime.Now;
                    await _walletRepo.UpdateAsync(wallet);
                }

                return isSuccess;
            }
            catch
            {
                return false;
            }
        }
    }
}