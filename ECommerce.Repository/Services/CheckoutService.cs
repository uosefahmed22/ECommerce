using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Errors;
using ECommerce.Core.IRepositories;
using ECommerce.Core.IServices;
using ECommerce.Core.Models;
using ECommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly AppDbContext _dbContext;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;
        private readonly IShoppingCartRepository _shoppingCartRepo;
        private readonly StripeClient _client;
        private const string CartPrefix = "cart_";

        public CheckoutService(AppDbContext dbContext
            , IRedisCacheService redisCacheService
            ,IMapper mapper
            ,IShoppingCartRepository shoppingCartRepo)
        {
            _dbContext = dbContext;
            _redisCacheService = redisCacheService;
            _mapper = mapper;
            _shoppingCartRepo = shoppingCartRepo;
            _client = new StripeClient("sk_test_51OEfGwISfR8okMImo3YoeSYSnf9BKPQmf9AQDVNBl1ol4dFH3DvTthAqzWawgSAdNOwStiEBskdk8DbBJVjv42gh00TdtTmGsv");

        }
        public async Task<ApiResponse> CreateCheckoutAsync(string userId, string cartId, OrderModelDto orderModelDto)
        {
            try
            {
                // 1. Retrieve the user and validate the cart
                var user = await _dbContext.Users.FindAsync(userId);
                var cartKey = $"{CartPrefix}{userId}";
                if (cartKey is null || user is null)
                {
                    return new ApiResponse(400, "User does not have a cart.");
                }

                // 2. Fetch cart items from the repository
                var shoppingCartRepo = await _shoppingCartRepo.GetCartItemsAsync(userId);
                if (shoppingCartRepo is null || shoppingCartRepo.Data is not List<CartItem> cartItems)
                {
                    return new ApiResponse(400, "Cart is empty.");
                }

                // 3. Calculate the total amount for all cart items
                var totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

                // 4. Configure Stripe session options for payment
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    LineItems = cartItems.Select(item => new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // Convert price to cents
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Cart Item",
                            }
                        },
                        Quantity = (long)item.Quantity,
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = "https://uosefahmed22.github.io/ECommerceInAngular/Orders",
                    CancelUrl = "https://uosefahmed22.github.io/ECommerceInAngular/Orders/Cart",
                };

                // 5. Create Stripe payment session
                var service = new SessionService(_client);
                var session = service.Create(options);

                if (session is null)
                {
                    return new ApiResponse(400, "Failed to create payment session.");
                }

                // 6. Create the order object
                var order = _mapper.Map<OrderModel>(orderModelDto);
                order.UserId = userId;
                order.ItemsPurchased = cartItems.Select(item => item.ProductId.ToString()).ToList();
                order.TotalPrice = totalAmount;

                // 7. Begin database transaction
                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // 7.1 Update TotalSold and Stock for each product
                    foreach (var item in cartItems)
                    {
                        var product = await _dbContext.products.FindAsync(item.ProductId);
                        if (product is not null)
                        {
                            // Check if stock is sufficient
                            if (product.Stock < item.Quantity)
                            {
                                return new ApiResponse(400, "Insufficient stock for one or more products.");
                            }

                            // Update stock and total sold
                            product.TotalSold += (int)item.Quantity;
                            product.Stock -= item.Quantity;
                        }
                    }

                    // 7.2 Add the order to the database
                    await _dbContext.orders.AddAsync(order);
                    await _dbContext.SaveChangesAsync();

                    // 7.3 Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of failure
                    await transaction.RollbackAsync();
                    return new ApiResponse(500, $"Error processing the order: {ex.Message}");
                }

                // 8. Clear the cart from Redis cache
                await _shoppingCartRepo.clearCartAsync(userId);

                return new ApiResponse(200, "Order created successfully.", new
                {
                    OrderId = order.Id,
                    TotalAmount = totalAmount,
                    PaymentUrl = session.Url
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Unexpected error: {ex.Message}");
            }
        }

        public async Task<List<OrderSummaryDto>> GetAllCheckoutsOfUserAsync(string userId)
        {
            try
            {
                // 1. Validate user existence
                var user = await _dbContext.Users.FindAsync(userId);
                if (user is null)
                {
                    throw new Exception("User does not exist");
                }

                // 2. Retrieve orders
                var orders = await _dbContext.orders
                                    .Where(o => o.UserId == userId)
                                    .ToListAsync();

                // 3. Build response with additional product details
                var orderSummaries = new List<OrderSummaryDto>();

                // 4. Retrieve product details for each order
                foreach (var order in orders)
                {
                    var productIds = order.ItemsPurchased.Select(int.Parse).ToList();

                    var products = await _dbContext.products
                                                   .Where(p => productIds.Contains(p.Id))
                                                   .ToListAsync();

                    var productDetails = products.Select(p => new ProductDetailsDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description,
                        ImageCoverUrl = p.ImageCoverUrl
                    }).ToList();

                    var orderSummary = new OrderSummaryDto
                    {
                        Id = order.Id,
                        Address = order.Address,
                        PhoneNumber = order.PhoneNumber,
                        City = order.City,
                        TotalPrice = order.TotalPrice,
                        OrderDate = order.OrderDate,
                        UserName = order.AppUser.FullName,
                        Email = order.AppUser.Email,
                        ProductDetails = productDetails
                    };
                    orderSummaries.Add(orderSummary);
                }
                return orderSummaries;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}