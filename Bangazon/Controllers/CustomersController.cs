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
//This controller allows you to get all customers, get a single customer, query the products the customer is selling, query the payment types the customer has used to pay, post a new customer, and edit an existing customer

namespace Bangazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
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

        //GET api/Customers
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

            //GET api/Customers?q=Taco
            if (q != null)
            {
                string isQ = $@"
                    AND c.FirstName LIKE '%{q}%'
                    OR c.LastName LIKE '%{q}%'
                ";
                sql = $"{sql} {isQ}";
            }

            //GET api/Customers?q=Taco&_include=products
            if (_include == "products")
            {
                string sqlproduct = @" 
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
                    LEFT JOIN Product p ON c.Id = p.CustomerId
                    WHERE 1=1
                ";
                if (q != null)
                {
                    string isQ = $@"
                    AND c.FirstName LIKE '%{q}%'
                    OR c.LastName LIKE '%{q}%'
                ";
                    sqlproduct = $"{sqlproduct} {isQ}";
                }
                Dictionary<int, Customer> productReport = new Dictionary<int, Customer>();

                Connection.Query<Customer, Product, Customer>(
                    sqlproduct,

                    (Customer, Product) => {
                        if (!productReport.ContainsKey(Customer.Id))
                        {
                            productReport[Customer.Id] = Customer;
                        }
                        productReport[Customer.Id].Products.Add(Product);

                        return Customer;
                    }
                );
                return Ok(productReport.Values);
            }

            //GET api/Customers?q=Taco&_include=payments
            if (_include == "payments")
            {
                string sqlpayment = @" 
                    SELECT 
                        c.Id,
                        c.FirstName,
                        c.LastName,
                        pt.Id,
                        pt.AcctNumber,
                        pt.Name,
                        pt.CustomerId
                    FROM Customer c
                    LEFT JOIN PaymentType pt ON c.Id = pt.CustomerId
                    WHERE 1=1
                ";
                if (q != null)
                {
                    string isQ = $@"
                    AND c.FirstName LIKE '%{q}%'
                    OR c.LastName LIKE '%{q}%'
                ";
                    sqlpayment = $"{sqlpayment} {isQ}";
                }
                Dictionary<int, Customer> paymentReport = new Dictionary<int, Customer>();

                Connection.Query<Customer, PaymentType, Customer>(
                   sqlpayment,

                    (Customer, PaymentType) => {
                        if (!paymentReport.ContainsKey(Customer.Id))
                        {
                            paymentReport[Customer.Id] = Customer;
                        }
                        paymentReport[Customer.Id].PaymentTypes.Add(PaymentType);

                        return Customer;
                    }
                );
                return Ok(paymentReport.Values);
            }

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(sql);
                return Ok(customers);
            }
        }

        // GET api/customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get([FromRoute]int id, string _include)
        {
            string sql = $@"
            SELECT
                c.Id,
                c.FirstName,
                c.LastName
            FROM Customer c
            WHERE c.Id = {id}
            ";

            if (_include == "products")
            {
                string sqlproduct = $@" 
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
                    LEFT JOIN Product p ON c.Id = p.CustomerId
                    WHERE c.Id = {id}
                ";

                
                
                Dictionary<int, Customer> productReport = new Dictionary<int, Customer>();

                Connection.Query<Customer, Product, Customer>(
                    sqlproduct,

                    (Customer, Product) => {
                        if (!productReport.ContainsKey(Customer.Id))
                        {
                            productReport[Customer.Id] = Customer;
                        }
                        productReport[Customer.Id].Products.Add(Product);

                        return Customer;
                    }
                );
                return Ok(productReport.Values.Single());
            }

            if (_include == "payments")
            {
                string sqlpayment = $@" 
                    SELECT 
                        c.Id,
                        c.FirstName,
                        c.LastName,
                        pt.Id,
                        pt.AcctNumber,
                        pt.Name,
                        pt.CustomerId
                    FROM Customer c
                    LEFT JOIN PaymentType pt ON c.Id = pt.CustomerId
                    WHERE c.Id = {id}
                ";
                
                Dictionary<int, Customer> paymentReport = new Dictionary<int, Customer>();

                Connection.Query<Customer, PaymentType, Customer>(
                   sqlpayment,

                    (Customer, PaymentType) => {
                        if (!paymentReport.ContainsKey(Customer.Id))
                        {
                            paymentReport[Customer.Id] = Customer;
                        }
                        paymentReport[Customer.Id].PaymentTypes.Add(PaymentType);

                        return Customer;
                    }
                );
                return Ok(paymentReport.Values.Single());
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(sql);
                return Ok(customers.Single());
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
