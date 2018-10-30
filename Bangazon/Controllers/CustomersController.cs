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

namespace Bangazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
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

        //GET api/Customers?q=Taco
        [HttpGet]
        public async Task<IActionResult> Get(string q, string _include)
        {
            string sql = @"
            SELECT
                c.Id,
                c.FirstName,
                c.LastName
            FROM Customer c
            WHERE 1=1
            ";

            if (q != null)
            {
                string isQ = $@"
                    AND c.FirstName LIKE '%{q}%'
                    OR c.LastName LIKE '%{q}%'
                ";
                sql = $"{sql} {isQ}";
            }

            if (_include == "products")
            {
                Dictionary<int, Customer> report = new Dictionary<int, Customer>();

                Connection.Query<Customer, Product, Customer>(
                    @" 
                    SELECT 
                        c.Id,
                        c.FirstName,
                        c.LastName,
                        p.Id,
                        p.ProductTypeId,
                        p.CustomerId,
                        p.Price,
                        p.Title,
                        p.Description,
                        p.Quantity
                    FROM Customer c
                    JOIN Product p ON c.Id = p.CustomerId
                ",
                    (Customer, Product) => {
                        if (!report.ContainsKey(Customer.Id))
                        {
                            report[Customer.Id] = Customer;
                        }
                        report[Customer.Id].Products.Add(Product);

                        return Customer;
                    }
                );
                return Ok(report.Values);
            }

            Console.WriteLine(sql);

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(sql);
                return Ok(customers);
            }
        }

        // GET api/customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                c.Id,
                c.FirstName,
                c.LastName
            FROM Customer c
            WHERE c.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(sql);
                return Ok(customers);
            }
        }

        // POST api/customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            string sql = $@"INSERT INTO Customer 
            (FirstName, LastName)
            VALUES
            (
                '{customer.FirstName}',
                '{customer.LastName}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                customer.Id = newId;
                return CreatedAtRoute("GetCustomer", new { id = newId }, customer);
            }
        }

        // PUT api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            string sql = $@"
            UPDATE Customer
            SET FirstName = '{customer.FirstName}',
                LastName = '{customer.LastName}'
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
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CustomerExists(int id)
        {
            string sql = $"SELECT Id FROM Customer WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Customer>(sql).Count() > 0;
            }
        }
    }
}
