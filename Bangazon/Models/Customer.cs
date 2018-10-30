﻿using System.Collections.Generic;

namespace Bangazon.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
