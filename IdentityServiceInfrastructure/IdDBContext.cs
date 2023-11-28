using IdentityServiceDomain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceInfrastructure
{
    public class IdDBContext : IdentityDbContext<User, Role, Guid>
    {
        public IdDBContext(DbContextOptions<IdDBContext> options)
            : base(options) { }

        public IdDBContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection_str = Environment.GetEnvironmentVariable("DefaultDB:ConnStr");
            optionsBuilder.UseSqlServer(connection_str);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Entity<User>().HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
