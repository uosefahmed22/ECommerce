using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class CartItem
    {
        public string CartId { get; set; }
        public string CartOwnerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPriceOfItem { get; set; }
        public string? ImageCoverUrl { get; set; }
        public DateTime TimeAdded { get; set; } = DateTime.Now;
        public void CalculateTotalPriceOfItem()
        {
            TotalPriceOfItem = Quantity * Price;
        }
    }
}
