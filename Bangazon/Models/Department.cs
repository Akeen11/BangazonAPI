using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Gretchen Ward
//this is the model for department

namespace Bangazon.Models
{
    public class Department
    {
        public int Id { get; set; }
      
        public string Name { get; set; }

        public int Budget { get; set; }
    }
}
