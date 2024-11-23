using ECommerce.Core.Errors;
using ECommerce.Core.IRepositories;
using ECommerce.Core.IServices;
using ECommerce.Core.Models;
using ECommerce.Core.ReponseModels;
using ECommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly AppDbContext _dbContext;
        private const string CartPrefix = "cart_";

        public ShoppingCartRepository(IRedisCacheService redisCacheService, AppDbContext dbContext)
        {
            _redisCacheService = redisCacheService;
            _dbContext = dbContext;
        }

        public async Task<ApiResponse> AddToCartAsync(string userId, int productId)
        {
            try
            {
                // Get cart key
                var cartKey = $"{CartPrefix}{userId}";

                // Retrieve the cart from Redis or initialize a new one
                var cart = await _redisCacheService.GetCacheValueAsync<Cart>(cartKey)
                    ?? new Cart
                    {
                        CartId = Guid.NewGuid().ToString(), // Ensure CartId is set here
                        OwnerId = userId,
                        Createdat = DateTime.UtcNow,
                        Updatedat = DateTime.UtcNow,
                        Items = new List<CartItem>()
                    };

                // Ensure Items property is initialized
                cart.Items ??= new List<CartItem>();

                // Check if the product exists in the database
                var product = await _dbContext.products.FindAsync(productId);
                if (product == null)
                {
                    return new ApiResponse(404, "Product not found");
                }

                // Check if the product is already in the cart
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    return new ApiResponse(400, "Product already exists in cart");
                }

                // Add the new product to the cart
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    TimeAdded = DateTime.UtcNow,
                    CartOwnerId = userId,
                    CartId = cart.CartId // Ensure CartId is passed to the cart item
                });

                // Update cart timestamp
                cart.Updatedat = DateTime.UtcNow;

                // Save the updated cart back to Redis
                await _redisCacheService.SetCacheValueAsync(cartKey, cart, TimeSpan.FromDays(7));

                // Return a successful response
                return new ApiResponse(200, "Product added to cart successfully");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ProductCartApiResponse> GetCartItemsAsync(string userId)
        {
            try
            {
                // 1. Validate user existence and cart key
                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ProductCartApiResponse(400, "User does not exist or does not have a cart.");
                }

                var cartKey = $"{CartPrefix}{userId}";

                // 2. Retrieve cart from Redis cache
                var cart = await _redisCacheService.GetCacheValueAsync<Cart>(cartKey);
                if (cart == null || cart.Items == null || !cart.Items.Any())
                {
                    return new ProductCartApiResponse(400, "Cart is empty or not found.");
                }

                // 3. Fetch product details from the database
                var productIds = cart.Items.Select(item => item.ProductId).Distinct().ToList();
                var products = await _dbContext.products
                                               .Where(p => productIds.Contains(p.Id))
                                               .ToListAsync();
                if (products == null || !products.Any())
                {
                    return new ProductCartApiResponse(404, "No products found for cart items.");
                }

                // 4. Map cart items with product details and calculate total price
                var cartItemsDetails = cart.Items.Select(item =>
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null)
                        return null;

                    return new CartItem
                    {
                        CartId = cart.CartId,
                        CartOwnerId = item.CartOwnerId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ImageCoverUrl = product.ImageCoverUrl,
                        Price = product.Price, // Use product price from the database
                        TotalPriceOfItem = item.Quantity * product.Price, // Calculate total price
                        TimeAdded = item.TimeAdded
                    };
                }).Where(item => item != null).ToList();

                // 5. Return response
                return new ProductCartApiResponse(200, "Cart items retrieved successfully", cartItemsDetails.Count, cartItemsDetails);
            }
            catch (Exception ex)
            {
                // Return error response with exception message
                return new ProductCartApiResponse(500, ex.Message);
            }
        }

        public async Task<ApiResponse> clearCartAsync(string userId)
        {
            try
            {
                // 1. check if user exsits & have a cart
                var user = await _dbContext.Users.FindAsync(userId);
                var cartKey = $"{CartPrefix}{userId}";
                if (cartKey == null || user == null)
                {
                    return new ApiResponse(400, "User may not have a cart or does not exist");
                }

                // 3. clear cart
                await _redisCacheService.ClearCacheValueAsync(cartKey);

                return new ApiResponse(200, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> removeItemfromCartAsync(string userId, int productId)
        {
            try
            {
                // 1. check if user exsits & have a cart
                var user = await _dbContext.Users.FindAsync(userId);
                var cartKey = $"{CartPrefix}{userId}";
                if (cartKey == null || user == null)
                {
                    return new ApiResponse(400, "User may not have a cart or does not exist");
                }

                // 2. get cart from redis cache
                var cart = await _redisCacheService.GetCacheValueAsync<Cart>(cartKey);

                // 3. remove item from cart
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                {
                    return new ApiResponse(400, "Product not found in cart");
                }
                cart.Items.Remove(item);
                cart.Updatedat = DateTime.UtcNow;
                return new ApiResponse(200, "Product removed from cart successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }
        public async Task<ApiResponse> updateCartItemAsync(string userId, int productId, int quantity)
        {
            try
            {
                // Validate quantity
                if (quantity < 1)
                {
                    return await removeItemfromCartAsync(userId, productId); // Remove item if quantity is less than 1
                }

                // 1. Check if user exists & has a cart
                var user = await _dbContext.Users.FindAsync(userId);
                var cartKey = $"{CartPrefix}{userId}";
                if (user == null)
                {
                    return new ApiResponse(400, "User does not exist");
                }

                // 2. Retrieve cart from Redis cache
                var cart = await _redisCacheService.GetCacheValueAsync<Cart>(cartKey);
                if (cart == null || cart.Items == null)
                {
                    return new ApiResponse(400, "Cart not found");
                }

                // 3. Find the item to update
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                {
                    return new ApiResponse(400, "Product not found in cart");
                }

                // 4. Update item quantity and total price
                var product = await _dbContext.products.FindAsync(productId);
                if (product == null)
                {
                    return new ApiResponse(404, "Product not found in database");
                }

                item.Quantity = quantity;
                item.Price = product.Price; // Ensure the price is updated from the database
                item.CalculateTotalPriceOfItem(); // Recalculate total price using the method
                cart.Updatedat = DateTime.UtcNow;

                // 5. Save the updated cart back to Redis
                await _redisCacheService.SetCacheValueAsync(cartKey, cart, TimeSpan.FromDays(7));

                return new ApiResponse(200, "Product updated in cart successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }
        }

    }
}