using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ProductDetailsDto> ProductDetails { get; set; } 
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

}