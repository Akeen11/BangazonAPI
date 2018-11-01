using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Http;
using Bangazon.Models;

//Aaron Keen
//

namespace Bangazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {

        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSupervisor,
                d.Id,
                d.Name,
                d.Budget,
                c.Id,
                c.PurchaseDate,
                c.DecomissionDate
            FROM Employee e
            LEFT JOIN Department d ON e.DepartmentId = d.Id
            LEFT JOIN ComputerEmployee ce ON e.Id = ce.EmployeeId
            LEFT JOIN Computer c ON ce.ComputerId = c.Id
            WHERE 1=1
            ";


            using (IDbConnection conn = Connection)
            {

                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Computer, Employee>(
                sql,
                (employee, department, computer) =>
                {
                    employee.Department = department;
                    employee.Computer = computer;
                    return employee;
                }
                );
                return Ok(employees);
            }
        }

        // GET api/Employees/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSupervisor,
                d.Id,
                d.Name,
                d.Budget,
                c.Id,
                c.PurchaseDate,
                c.DecomissionDate
            FROM Employee e
            LEFT JOIN Department d ON e.DepartmentId = d.Id
            LEFT JOIN ComputerEmployee ce ON e.Id = ce.EmployeeId
            LEFT JOIN Computer c ON ce.ComputerId = c.Id
            WHERE e.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Computer, Employee>(
                sql,
                (employee, department, computer) =>
                {
                    employee.Department = department;
                    employee.Computer = computer;
                    return employee;
                }
                );
                return Ok(employees);
            }
        }

        // POST api/Employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            string sql = $@"INSERT INTO Employee 
            (FirstName, LastName, DepartmentId, IsSupervisor)
            VALUES
            (
                '{employee.FirstName}',
                '{employee.LastName}',
                '{employee.DepartmentId}',
                '{employee.IsSupervisor}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                employee.Id = newId;
                return CreatedAtRoute("GetCustomer", new { id = newId }, employee);
            }
        }

        // PUT api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            string sql = $@"
            UPDATE Employee
            SET FirstName = '{employee.FirstName}',
                LastName = '{employee.LastName}',
                DepartmentId = '{employee.DepartmentId}',
                IsSupervisor = '{employee.IsSupervisor}'
            WHERE Id = {id}";

            try
            {
                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                    }
                    throw new Exception("No rows affected");
                }
            }
            catch (Exception)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM Employee WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                if (rowsAffected > 0)
                {
                    return new StatusCodeResult(StatusCodes.Status204NoContent);
                }
                throw new Exception("No rows affected");
            }

        }

        private bool EmployeeExists(int id)
        {
            string sql = $"SELECT Id FROM Employee WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Employee>(sql).Count() > 0;
            }
        }
    }
}
