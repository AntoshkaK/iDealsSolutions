using iDealsSolutions.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace iDealsSolutions.DbData
{
    public class DBContext : DbContext
    {
        public DBContext()
            : base("DBConnection")
        {
        }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Logs> Logs { get; set; }

        public Logs Log;
        public static bool ConnectionCheck()
        {           
            var dbContext = new DBContext();
            using (var connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
            return true;
        }
        public static DBContext Create()
        {
            return new DBContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }     
       
    }   
}