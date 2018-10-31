﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
//Gretchen
//model for ComputerEmployee
namespace Bangazon.Models
{
    public class ComputerEmployee
    {
        
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int ComputerId { get; set; }

        public DateTime AssignDate { get; set; }
      
        public DateTime UnassignDate { get; set; }

    }
}