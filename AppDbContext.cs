using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrudApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudApp
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Item>? Items { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=app.db"); // SQLite database file
            }
        }
    }
}