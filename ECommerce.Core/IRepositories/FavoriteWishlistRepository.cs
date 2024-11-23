using ECommerce.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.IRepositories
{
    public interface IFavoriteWishlistRepository
    {
        // Favorite
        Task<ApiResponse> GetFavoritesByUserIdAsync(string userId);
        Task<ApiResponse> AddFavoriteAsync(string userId, int productId);
        Task<ApiResponse> RemoveFavoriteAsync(string userId, int productId);

        // Wishlist
        Task<ApiResponse> GetWishlistByUserIdAsync(string userId);
        Task<ApiResponse> AddWishlistItemAsync(string userId, int productId);
        Task<ApiResponse> RemoveWishlistItemAsync(string userId, int productId);
    }
}
