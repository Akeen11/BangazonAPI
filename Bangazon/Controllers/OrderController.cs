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

        // GET: api/<controller> to get all orders, only completed ones, or only not completed ones 
        [HttpGet]
        public async Task<IActionResult> Get(bool? _completed, string q, string _include)
        {
            //this sets sql as an empty string 
            string sql = "";
            
            //if _completed is null fill the sql string with this
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
            //if _completed is NULL(http://localhost:5000/api/Order) fill it with this
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
                //if _completed=false(http://localhost:5000/api/Order?_completed=false) add this to the sql statement
                if (_completed == false)
                {
                    string isComplete = @"
                        AND o.PaymentTypeId IS NULL
                    ";
                    sql = $"{sql} {isComplete}";
                }
                //if _completed=true(http://localhost:5000/api/Order?_completed=true) ad this to the sql statement 
                else
                {
                    string isComplete = @"
                        AND o.PaymentTypeId IS NOT NULL
                    ";
                    sql = $"{sql} {isComplete}";
                }
            }
            if (_include == "customer") 
            {
                sql = $@"
                    SELECT 
                        o.Id,
                        o.CustomerId,
                        o.PaymentTypeId,
                        c.Id,
                        c.FirstName,
                        c.LastName
                    FROM [Order] o
                    JOIN Customer c ON c.Id = o.CustomerId
                    WHERE 1=1
                ";
                using (IDbConnection conn = Connection)
                {
                    IEnumerable<Order> orders = await conn.QueryAsync<Order, Customer, Order>(
                        sql,
                        (order, customer) => {
                            order.Customer = customer;
                            return order;
                        }
                        );
                    return Ok(orders);
                }
            }

            if (_include == "product")
            {
                sql = $@"
                    SELECT 
                        o.Id,
                        o.CustomerId,
                        o.PaymentTypeId,
                        p.Id,
                        p.ProductTypeId,
                        p.CustomerId,
                        p.Title,
                        p.Description,
                        p.Quantity
                    FROM [Order] o 
                    LEFT JOIN OrderProduct op ON o.Id = op.OrderId
                    LEFT JOIN Product p ON op.ProductId = p.Id
                    WHERE 1=1
                ";

                Dictionary<int, Order> productList = new Dictionary<int, Order>();
                Connection.Query<Order, Product, Order>(
                    sql,
                    (Order, Product) =>
                    {
                        if (!productList.ContainsKey(Order.Id))
                        {
                            productList[Order.Id] = Order;
                        }
                        productList[Order.Id].products.Add(Product);
                        return Order;
                    }
                    );
                    return Ok(productList.Values);
                
            }


            using (IDbConnection conn = Connection)
            {
                IEnumerable<Order> orders = await conn.QueryAsync<Order>(
                    sql);
                        return Ok(orders);
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
            string sql = $@"
            DELETE FROM OrderProduct WHERE OrderId = {id};
            DELETE FROM [Order] WHERE Id = {id};";

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
