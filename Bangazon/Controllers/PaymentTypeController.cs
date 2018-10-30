using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Bangazon.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

//Gretchen Ward
// This controller allows the get all payment types, get a single payment type, post a new payment type, put(edit) an existing payment type, and delete a payment type


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace Bangazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentTypeController(IConfiguration config)
        {
            _config = config;
        }


        //Connection to Database--Method
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        //Get api/paymentType
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = "SELECT * FROM PaymentType";
                var thisPaymentType = (await conn.QueryAsync<PaymentType>(sql));
                return Ok(thisPaymentType);
            }
        }

        // GET api/PaymentType/1
        //returns a single item using .Single
        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $"SELECT * FROM PaymentType WHERE Id = {id}";

                var OnePaymentType = await conn.QueryAsync<PaymentType>(sql);
                return Ok(OnePaymentType.Single());
            }
        }


        // POST api/PaymentType/Post
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentType paymentType)
        {

            string sql = $@"INSERT INTO PaymentType
            (AcctNumber, Name, CustomerId)
            VALUES
            ('{paymentType.AcctNumber}', '{paymentType.Name}','{paymentType.CustomerId}');
           SELECT SCOPE_IDENTITY()  MAX(Id) from PaymentType";

            using (IDbConnection conn = Connection)
            {
                var newPaymentTypeId = (await conn.QueryAsync<int>(sql)).Single();
                paymentType.Id = newPaymentTypeId;
                return CreatedAtRoute("GetPaymentType", new { id = newPaymentTypeId }, paymentType);
            }

        }

        // PUT api/PaymentType/Put
        //Put method changes a current object
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PaymentType paymentType)
        {
            string sql = $@"
            UPDATE PaymentType
                 
            SET   AcctNumber = '{paymentType.AcctNumber}'
                  Name = '{paymentType.Name}'
                  CustomerId ='{paymentType.CustomerId}'

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
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        // DELETE api/PaymentType/Delete
        //Delete method finds payment type by id 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            string sql = $@"DELETE FROM PaymentType WHERE Id = {id}";
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

        private bool PaymentTypeExists(int id)
        {
            string sql = $"SELECT AcctNumber, Name, CustomerId FROM PaymentType WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<PaymentType>(sql).Count() > 0;
            }
        }

    }
}

