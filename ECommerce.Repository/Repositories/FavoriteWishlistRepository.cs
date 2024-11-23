using ECommerce.Core.Errors;
using ECommerce.Core.IRepositories;
using ECommerce.Core.Models;
using ECommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class FavoriteWishlistRepository : IFavoriteWishlistRepository
    {
        private readonly AppDbContext _dbContext;

        public FavoriteWishlistRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Favorite
        public async Task<ApiResponse> AddFavoriteAsync(string userId, int productId)
        {
            try
            {
                if (await _dbContext.favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId))
                {
                    return new ApiResponse(400, "Product already in favorites");
                }
                var favorite = new Favorite
                {
                    UserId = userId,
                    ProductId = productId
                };
                await _dbContext.favorites.AddAsync(favorite);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "Favorite added successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> GetFavoritesByUserIdAsync(string userId)
        {
            try
            {
                var favorites =await _dbContext
                    .favorites
                    .Where(f => f.UserId == userId)
                    .Include(f => f.Product)
                    .Select(f => new
                    {
                        f.ProductId,
                        f.Product.Name,
                        f.Product.Price,
                        f.Product.Description,
                        f.Product.ImageCoverUrl
                    })
                    .ToListAsync();
                return new ApiResponse(200, favorites);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> RemoveFavoriteAsync(string userId, int productId)
        {
            try
            {
                var favorite = await _dbContext.favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
                if (favorite == null)
                {
                    return new ApiResponse(404, "Favorite not found");
                }
                _dbContext.favorites.Remove(favorite);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "Favorite removed successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }


        }

        // Wishlist
        public async Task<ApiResponse> AddWishlistItemAsync(string userId, int productId)
        {
            try
            {
                if (await _dbContext.wishlists.AnyAsync(w => w.UserId == userId && w.ProductId == productId))
                {
                    return new ApiResponse(400, "Product already in wishlist");
                }
                var wishlist = new Wishlist
                {
                    UserId = userId,
                    ProductId = productId
                };
                await _dbContext.wishlists.AddAsync(wishlist);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "Wishlist item added successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> GetWishlistByUserIdAsync(string userId)
        {
            try
            {
                var wishlist = await _dbContext
                    .wishlists
                    .Where(w => w.UserId == userId)
                    .Include(w => w.Product)
                    .Select(w => new
                    {
                        w.ProductId,
                        w.Product.Name,
                        w.Product.Price,
                        w.Product.Description,
                        w.Product.ImageCoverUrl
                    })
                    .ToListAsync();
                return new ApiResponse(200, wishlist);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> RemoveWishlistItemAsync(string userId, int productId)
        {
            try
            {
                var wishlist = await _dbContext
                    .wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
                if (wishlist == null)
                {
                    return new ApiResponse(404, "Wishlist item not found");
                }
                _dbContext.wishlists.Remove(wishlist);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse(200, "Wishlist item removed successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
    }
}
