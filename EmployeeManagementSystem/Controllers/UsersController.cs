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
using EmployeeManagementSystem.DTOs.UserDTOs;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EmployeeManagementSystemContext _context;

        public UsersController(EmployeeManagementSystemContext context)
        {
            _context = context;
        }

        // POST: api/Users/register
        //[HttpPost("register")]
        //public async Task<IActionResult> RegisterUser([FromBody] User user)
        //{
        //    var result = await _context.Database.ExecuteSqlRawAsync(
        //        "EXEC [dbo].[RegisterUser] @Username = {0}, @Password = {1}", user.Username, user.Password
        //        );

        //    if (result == 1)
        //        return Ok("User registration successfully.");
        //    else
        //        return Conflict("Username already exists.");
        //}

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[RegisterUser] @Username = {0}, @Password = {1}",
                    user.Username, user.Password
                );

                return Ok("User registration successfully.");
            }
            catch (SqlException ex) when (ex.Number == 51000)
            {
                return Conflict("Username already exists.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }



        //// POST: api/Users/login
        //[HttpPost("login")]
        //public async Task<IActionResult> LoginUser([FromBody] User user)
        //{
        //    var matchedUser = await _context.User.FromSqlRaw("SELECT * FROM Users WHERE Username = {0} AND Password = {1}", user.Username, user.Password).FirstOrDefaultAsync();

        //    if (matchedUser != null)
        //        return Ok("Login Successful.");
        //    else
        //        return Unauthorized("Invalid username or password.");
        //}


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] User user)
        {
            var parameters = new[]
            {
        new SqlParameter("@Username", user.Username),
        new SqlParameter("@Password", user.Password)
    };

            try
            {
                var result = await _context.Set<UserLoginDTO>()
                    .FromSqlRaw("EXEC dbo.LoginUser @Username, @Password", parameters)
                    .ToListAsync();

                var matchedUser = result.FirstOrDefault();

                return Ok(new
                {
                    message = "Login successful.",
                    user = matchedUser
                });
            }
            catch (SqlException ex) when (ex.Number == 51001)
            {
                return Unauthorized("Invalid username or password.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }

    }
