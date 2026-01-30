using StudentManagementSystem.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IVnPayService
    {
        // Accept client IP instead of HttpContext
        string CreatePaymentUrl(VnPayPaymentRequest request, string clientIp);

        // Accept a simple dictionary for query params instead of IQueryCollection
        VnPayCallbackModel ProcessCallback(IDictionary<string, string> queryParams);

        Task<bool> ProcessPaymentAsync(VnPayCallbackModel callback);
    }
}