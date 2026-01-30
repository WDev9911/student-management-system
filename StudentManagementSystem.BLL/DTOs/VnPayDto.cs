namespace StudentManagementSystem.BLL.DTOs
{
    public class VnPayPaymentRequest
    {
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string OrderType { get; set; } = "other";
        public string Locale { get; set; } = "vn";
    }

    public class VnPayPaymentResponse
    {
        public bool Success { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class VnPayCallbackModel
    {
        public string vnp_TmnCode { get; set; } = string.Empty;
        public string vnp_Amount { get; set; } = string.Empty;
        public string vnp_BankCode { get; set; } = string.Empty;
        public string vnp_BankTranNo { get; set; } = string.Empty;
        public string vnp_CardType { get; set; } = string.Empty;
        public string vnp_PayDate { get; set; } = string.Empty;
        public string vnp_OrderInfo { get; set; } = string.Empty;
        public string vnp_TransactionNo { get; set; } = string.Empty;
        public string vnp_ResponseCode { get; set; } = string.Empty;
        public string vnp_TransactionStatus { get; set; } = string.Empty;
        public string vnp_TxnRef { get; set; } = string.Empty;
        public string vnp_SecureHash { get; set; } = string.Empty;
    }

    public class WalletInfoDto
    {
        public int WalletId { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<TransactionDto> RecentTransactions { get; set; } = new();
    }

    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}