using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//gretchen
//model for employeeTraining
namespace Bangazon.Models
{
    public class EmployeeTraining
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int TrainingProgramId { get; set; }
    }
}
