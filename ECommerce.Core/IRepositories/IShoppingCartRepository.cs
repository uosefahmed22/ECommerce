using ECommerce.Core.Errors;
using ECommerce.Core.ReponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.IRepositories
{
    public interface IShoppingCartRepository
    {
        Task<ApiResponse> AddToCartAsync(string userId, int productId);
        Task<ProductCartApiResponse> GetCartItemsAsync(string userId);
        Task<ApiResponse> updateCartItemAsync(string userId, int productId, int quantity);
        Task<ApiResponse> removeItemfromCartAsync(string userId, int productId);
        Task<ApiResponse> clearCartAsync(string userId);
    }
}
