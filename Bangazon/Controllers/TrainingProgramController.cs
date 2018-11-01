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

// David Taylor
// This controller allows the user to get all training programs with all employees and filter by completed, get a single training program with all employees and filted by completed, post a new training program, put(edit) an existing training program, and delete a training program if it is not already completed.

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bangazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingProgramController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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

        // GET api/trainingprograms?_include=employees
        // GET api/trainingprograms?completed
        [HttpGet]
        public async Task<IActionResult> Get(string _include, bool completed)
        {
            string sql = @"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees
                FROM TrainingProgram tp
                WHERE 1=1
                ";
            if (_include != null && _include == "employees")
            {
                string sqlTraining = @"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSuperVisor
                FROM TrainingProgram tp
                LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
                LEFT JOIN Employee e ON et.EmployeeId = e.Id
             WHERE 1=1
            ";

                using (IDbConnection conn = Connection)
                {
                    Dictionary<int, TrainingProgram> trainingDictionary = new Dictionary<int, TrainingProgram>();

                    Connection.Query<TrainingProgram, Employee, TrainingProgram>(
                    sqlTraining,

                        (trainingProgram, employee) => {
                            if (!trainingDictionary.ContainsKey(trainingProgram.Id))
                            {
                                trainingDictionary[trainingProgram.Id] = trainingProgram;
                            }
                            trainingDictionary[trainingProgram.Id].Employees.Add(employee);

                            return trainingProgram;
                        }
                     );
                   return Ok(trainingDictionary.Values);
                }
            }
            if (completed != null && completed == false){
                string sqlTraining = @"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSuperVisor
                FROM TrainingProgram tp
                LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
                LEFT JOIN Employee e ON et.EmployeeId = e.Id
             WHERE StartDate >=  CONVERT(DATETIME, {fn CURDATE()})
            ";

                using (IDbConnection conn = Connection)
                {
                    Dictionary<int, TrainingProgram> trainingDictionary = new Dictionary<int, TrainingProgram>();

                    Connection.Query<TrainingProgram, Employee, TrainingProgram>(
                    sqlTraining,

                        (trainingProgram, employee) =>
                        {
                            if (!trainingDictionary.ContainsKey(trainingProgram.Id))
                            {
                                trainingDictionary[trainingProgram.Id] = trainingProgram;
                            }
                            trainingDictionary[trainingProgram.Id].Employees.Add(employee);

                            return trainingProgram;
                        }
                     );
                    return Ok(trainingDictionary.Values);
                }
            } 
            else 
            {
            string sqlTraining = @"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSuperVisor
                FROM TrainingProgram tp
                LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
                LEFT JOIN Employee e ON et.EmployeeId = e.Id
             WHERE StartDate <=  CONVERT(DATETIME, {fn CURDATE()})
            ";

                using (IDbConnection conn = Connection)
                {
                    Dictionary<int, TrainingProgram> trainingDictionary = new Dictionary<int, TrainingProgram>();

                    Connection.Query<TrainingProgram, Employee, TrainingProgram>(
                    sqlTraining,

                        (trainingProgram, employee) =>
                        {
                            if (!trainingDictionary.ContainsKey(trainingProgram.Id))
                            {
                                trainingDictionary[trainingProgram.Id] = trainingProgram;
                            }
                            trainingDictionary[trainingProgram.Id].Employees.Add(employee);

                            return trainingProgram;
                        }
                        );
                    return Ok(trainingDictionary.Values);
                }
                
            }


                using (IDbConnection conn = Connection)
            {

                IEnumerable<TrainingProgram> trainingPrograms = await conn.QueryAsync<TrainingProgram>(sql);
                return Ok(trainingPrograms);
            }
        }

        // GET api/TrainingPrograms/5?_include=employees
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get([FromRoute]int id, string _include)
        {
            string sql = $@"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees
                FROM TrainingProgram tp
                WHERE tp.id = {id}
                ";
            if (_include != null && _include == "employees")
            {
                string sqlTraining = $@"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSuperVisor
                FROM TrainingProgram tp
                LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
                LEFT JOIN Employee e ON et.EmployeeId = e.Id
            WHERE tp.id = {id}
            ";

                using (IDbConnection conn = Connection)
                {
                    Dictionary<int, TrainingProgram> trainingDictionary = new Dictionary<int, TrainingProgram>();

                    Connection.Query<TrainingProgram, Employee, TrainingProgram>(
                    sqlTraining,

                        (trainingProgram, employee) => {
                            if (!trainingDictionary.ContainsKey(trainingProgram.Id))
                            {
                                trainingDictionary[trainingProgram.Id] = trainingProgram;
                            }
                            trainingDictionary[trainingProgram.Id].Employees.Add(employee);

                            return trainingProgram;
                        }
                     );
                    return Ok(trainingDictionary.Values);
                }
            }
            
            using (IDbConnection conn = Connection)
            {

                IEnumerable<TrainingProgram> trainingPrograms = await conn.QueryAsync<TrainingProgram>(sql);
                return Ok(trainingPrograms);
            }
        }

        // POST api/students
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            string sql = $@"INSERT INTO TrainingProgram
            (StartDate, EndDate, MaxAttendees)
            VALUES
            (
                '{trainingProgram.StartDate}'
                ,'{trainingProgram.EndDate}'
                ,'{trainingProgram.MaximumAttendees}'

            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                trainingProgram.Id = newId;
                return CreatedAtRoute("GetTrainingProgram", new { id = newId }, trainingProgram);
            }
        }

        // PUT api/students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TrainingProgram trainingProgram)
        {
            string sql = $@"
            UPDATE TrainingProgram
            SET StartDate = '{trainingProgram.StartDate}',
                EndDate = '{trainingProgram.EndDate}',
                MaxAttendees = '{trainingProgram.MaximumAttendees}'
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
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/TrainingPrograms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM TrainingProgram WHERE Id = {id} AND";

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

        private bool TrainingProgramExists(int id)
        {
            string sql = $"SELECT Id FROM TrainingProgram WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<TrainingProgram>(sql).Count() > 0;
            }
        }
    }

}
