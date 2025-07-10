using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.DTOs.UserDTOs;
using EmployeeManagementSystem.Helpers;

namespace EmployeeManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly EmployeeManagementSystemContext _context;

        public UsersController(EmployeeManagementSystemContext context)
        {
            _context = context;
        }

        // Default landing page
        [HttpGet("/")]
        public IActionResult LoginRegister(bool isRegistering = false)
        {
            ViewBag.IsRegistering = isRegistering;
            return View("LoginRegister", new User());
        }

        [HttpPost("/handle")]
        public async Task<IActionResult> HandleLoginRegister(User user, bool isRegistering)
        {
            Console.WriteLine($"Form submitted. isRegistering: {isRegistering}, Username: {user.Username}");

            if (isRegistering)
            {
                return await RegisterUser(user);
            }
            else
            {
                return await LoginUser(user);
            }
        }

        [NonAction]
        public async Task<IActionResult> RegisterUser(User user)
        {
            try
            {
                string hashedPassword = PasswordHasher.Hash(user.Password);

                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[RegisterUser] @Username = {0}, @Password = {1}",
                    user.Username, hashedPassword
                );

                TempData["Success"] = "User registered successfully.";
                return RedirectToAction("LoginRegister", new { isRegistering = false });
            }
            catch (SqlException ex) when (ex.Number == 51000)
            {
                TempData["Error"] = "Username already exists.";
                return RedirectToAction("LoginRegister", new { isRegistering = true });
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred.";
                return RedirectToAction("LoginRegister", new { isRegistering = true });
            }
        }

        [NonAction]
        public async Task<IActionResult> LoginUser(User user)
        {
            string hashedPassword = PasswordHasher.Hash(user.Password);

            var parameters = new[]
            {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", hashedPassword)
            };

            try
            {
                var result = await _context.Set<UserLoginDTO>()
                    .FromSqlRaw("EXEC dbo.LoginUser @Username, @Password", parameters)
                    .ToListAsync();

                var matchedUser = result.FirstOrDefault();

                if (matchedUser != null)
                {
                    TempData["Success"] = "Login successful.";
                    return RedirectToAction("Dashboard", "Home"); 
                }

                TempData["Error"] = "Invalid username or password.";
                return RedirectToAction("LoginRegister", new { isRegistering = false });
            }
            catch (SqlException ex) when (ex.Number == 51001)
            {
                TempData["Error"] = "Invalid username or password.";
                return RedirectToAction("LoginRegister", new { isRegistering = false });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction("LoginRegister", new { isRegistering = false });
            }
        }

        public IActionResult Logout()
        {
            // Redirect to login page
            return RedirectToAction("LoginRegister", new { isRegistering = false });
        }
    }
}
