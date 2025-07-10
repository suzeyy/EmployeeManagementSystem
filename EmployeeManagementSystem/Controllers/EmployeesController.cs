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
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
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

                TempData["Success"] = "Employee updated successfully.";
            }
            catch (SqlException ex) when (ex.Number == 51003)
            {
                TempData["Error"] = "Email already exists.";
            }
            catch (SqlException ex) when (ex.Number == 51004)
            {
                TempData["Error"] = "Phone number already exists.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred.";
            }
            return RedirectToAction("Dashboard", "Home");
        }


        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            var parameters = new[]
            {
                new SqlParameter("@FirstName", employee.FirstName),
        new SqlParameter("@LastName", employee.LastName),
        new SqlParameter("@Email", employee.Email),
        new SqlParameter("@Phone", employee.Phone),
        new SqlParameter("@Position", employee.Position)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.AddEmployee @FirstName, @LastName, @Email, @Phone, @Position",
                    parameters
                );

                TempData["Success"] = "Employee added successfully.";
            }
            catch (SqlException ex) when (ex.Number == 51003)
            {
                TempData["Error"] = "Email already exists.";
            }
            catch (SqlException ex) when (ex.Number == 51004)
            {
                TempData["Error"] = "Phone number already exists.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred.: {ex.Message}";
            }
            return RedirectToAction("Dashboard", "Home");
        }

        // DELETE: api/Employees/5
        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var parameter = new SqlParameter("@EmployeeId", id);

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.DeleteEmployee @EmployeeId", parameter);
                TempData["Success"] = "Employee deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 51005)
            {
                TempData["Error"] = "Employee not found.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred.";
            }
            return RedirectToAction("Dashboard", "Home");
        }
    }
}
