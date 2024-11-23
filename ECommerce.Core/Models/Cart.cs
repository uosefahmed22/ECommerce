using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class Cart
    {
        public string CartId { get; set; }
        public string OwnerId { get; set; }
        public List<CartItem> Items { get; set; }
        public DateTime? Createdat { get; set; } = DateTime.Now;
        public DateTime Updatedat { get; set; } = DateTime.Now;
        public decimal TotalPrice => Items.Sum(x => x.TotalPriceOfItem);
    }
}
