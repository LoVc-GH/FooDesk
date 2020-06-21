using FooDesk.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FooDesk.Data
{
    public class FooDeskContext: DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Server=localhost;Database=FooDesk;Trusted_Connection=True;MultipleActiveResultSets=true;");
    }
}
