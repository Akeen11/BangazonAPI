using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models
{
    public class Computer
    {
        
        public int Id { get; set; }

       
        public DateTime PurchaseDate { get; set; }


        public DateTime? DecommissionDate { get; set; }

        
       
    }
}