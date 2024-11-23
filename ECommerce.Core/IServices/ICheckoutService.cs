using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.IServices
{
    public interface ICheckoutService
    {
        Task<ApiResponse> CreateCheckoutAsync(string userId, string cartId, OrderModelDto orderModelDto);
        Task<List<OrderSummaryDto>> GetAllCheckoutsOfUserAsync(string userId);
    }
}
