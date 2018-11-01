using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//gretchen
//model for order
namespace Bangazon.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int? PaymentTypeId { get; set; }

        public Customer Customer { get; set; }

        public List<Product> products { get; set; } = new List<Product>();
    }
}
