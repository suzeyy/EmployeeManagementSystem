using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmployeeManagementSystemContext _context;

        public HomeController(EmployeeManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var employees = await _context.Employee
                .FromSqlRaw("EXEC dbo.GetAllEmployees")
                .ToListAsync();

            return View(employees);
        }
    }
}
