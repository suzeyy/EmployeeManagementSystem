using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.Data.SqlClient;

namespace EmployeeManagementSystem.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class EmployeesController : Controller
    {
        private readonly EmployeeManagementSystemContext _context;

        public EmployeesController(EmployeeManagementSystemContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _context.Set<Employee>()
                    .FromSqlRaw("EXEC dbo.GetAllEmployees")
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to retrieve employee list.");
            }
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut()]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee)
        {
            var parameters = new[]
            {
        new SqlParameter("@EmployeeId", employee.EmployeeId),
        new SqlParameter("@FirstName", employee.FirstName),
        new SqlParameter("@LastName", employee.LastName),
        new SqlParameter("@Email", employee.Email),
        new SqlParameter("@Phone", employee.Phone),
        new SqlParameter("@Position", employee.Position)
    };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.UpdateEmployee @EmployeeId, @FirstName, @LastName, @Email, @Phone, @Position",
                    parameters
                );

                return Ok("Employee updated successfully.");
            }
            catch (SqlException ex) when (ex.Number == 51003)
            {
                return Conflict("Email already exists.");
            }
            catch (SqlException ex) when (ex.Number == 51004)
            {
                return Conflict("Phone number already exists.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            var parameters = new[]
            {
                new SqlParameter("@FirstName", employee.FirstName),
        new SqlParameter("@LastName", employee.LastName),
        new SqlParameter("@Email", employee.Email),
        new SqlParameter("@Phone", employee.Phone),
        new SqlParameter("@Position", employee.Position),
        new SqlParameter("@CreatedBy", employee.CreatedBy)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.AddEmployee @FirstName, @LastName, @Email, @Phone, @Position, @CreatedBy",
                    parameters
                );

                return Ok("Employee added successfully.");
            }
            catch (SqlException ex) when (ex.Number == 51003)
            {
                return Conflict("Email already exists.");
            }
            catch (SqlException ex) when (ex.Number == 51004)
            {
                return Conflict("Phone number already exists.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var parameter = new SqlParameter("@EmployeeId", id);

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.DeleteEmployee @EmployeeId", parameter);
                return Ok("Employee deleted successfully.");
            }
            catch (SqlException ex) when (ex.Number == 51005)
            {
                return NotFound("Employee not found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
