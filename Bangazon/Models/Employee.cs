using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//gretchen
//model for employee
namespace Bangazon.Models
{
    public class Employee
    {
       
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsSupervisor { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public List<Computer> Computer { get; set; } = new List<Computer>();

    }
}
