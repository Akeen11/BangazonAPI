using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//Kayla Reid

//User should be able to GET a list, and GET a single item.
//When an order is deleted, every line item (i.e.entry in OrderProduct) should be removed
//Should be able to filter out completed orders with the ? completed = false query string parameter.If the parameter value is true, then only completed order should be returned.
//  If the query string parameter of? _include = products is in the URL, then the list of products in the order should be returned.
//  If the query string parameter of? _include = customers is in the URL, then the customer representation should be included in the response.

namespace Bangazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
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

        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get(bool? _completed)
        {
            string sql = "";
            
            if (_completed == null)
            {
                sql = @"
                    SELECT 
                        o.Id,
                        o.CustomerId,
                        o.PaymentTypeId
                    FROM [Order] o
                ";
            }

            if (_completed != null)
            {
                 sql = $@"
                    SELECT 
                        o.Id,
                        o.CustomerId,
                        o.PaymentTypeId
                    FROM [Order] o
                    WHERE 1=1
                ";
                if (_completed == false)
                {
                    string isComplete = @"
                        AND o.PaymentTypeId IS NULL
                    ";
                    sql = $"{sql} {isComplete}";
                }
                else
                {
                    string isComplete = @"
                        AND o.PaymentTypeId IS NOT NULL
                    ";
                    sql = $"{sql} {isComplete}";
                }
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Order> order = await conn.QueryAsync<Order>(
                    sql);
                return Ok(order);
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
                SELECT 
                    o.Id,
                    o.CustomerId,
                    o.PaymentTypeId
                FROM [Order] o
                WHERE o.Id = {id}
            ";
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Order> order = await conn.QueryAsync<Order>(sql);
                return Ok(order.Single());
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
       
                string  sql = $@"
                  INSERT INTO [Order](CustomerId)
                  VALUES ({order.CustomerId})
                  SELECT SCOPE_IDENTITY();";
       
            Console.WriteLine(sql);

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                order.Id = newId;
                return CreatedAtRoute("GetOrder", new { id = newId }, order);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order order)
        {
            string sql = $@"
                UPDATE [Order] 
                SET CustomerId = {order.CustomerId},
                    PaymentTypeId = {order.PaymentTypeId}
                WHERE Id = {id}
            ";
            try
            {
                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return new StatusCodeResult(StatusCodes.Status200OK);
                    }

                    throw new Exception("No rows affected");
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM [Order] WHERE Id = {id}";

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

        private bool OrderExists(int id)
        {
            string sql = $"SELECT Id FROM [Order] WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<ProductType>(sql).Count() > 0;
            }
        }
    }
}
