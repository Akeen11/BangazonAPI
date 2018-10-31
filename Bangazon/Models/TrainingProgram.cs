using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//gretchen
//model for trainingprogram
namespace Bangazon.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
      
        public DateTime EndDate { get; set; }
    
        public int MaximumAttendees { get; set; }

        public int EmployeeTrainingId { get; set; }

        public int EmployeeId { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
