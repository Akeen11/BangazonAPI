using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
//Gretchen
//model for computer
namespace Bangazon.Models
{
    public class Computer
    {
        
        public int Id { get; set; }

       
        public DateTime PurchaseDate { get; set; }


        public DateTime DecomissionDate { get; set; }

    }
}