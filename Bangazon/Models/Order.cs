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

        // public Customer Customer { get; set; }

        public int CustomerId { get; set; }

        //use paymentTypeId to find out if order the is compleated 
        public int PaymentTypeId { get; set; }

        // public List<Product> productList { get; set; } = new List<Product>();
    }
}
