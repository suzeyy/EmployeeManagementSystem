using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.DTOs.UserDTOs;

namespace EmployeeManagementSystem.Data
{
    public class EmployeeManagementSystemContext : DbContext
    {
        public EmployeeManagementSystemContext (DbContextOptions<EmployeeManagementSystemContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeManagementSystem.Models.User> User { get; set; } = default!;
        public DbSet<EmployeeManagementSystem.Models.Employee> Employee { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLoginDTO>().HasNoKey();
        }
    }
}
