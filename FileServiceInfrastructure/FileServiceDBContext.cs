using FileServiceDomain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServiceInfrastructure
{
    public class FileServiceDBContext : DbContext
    {
        public DbSet<UploadedItem> UploadItems { get; private set; }

        public FileServiceDBContext() { }

        public FileServiceDBContext(DbContextOptions<FileServiceDBContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            string connection_str = Environment.GetEnvironmentVariable("DefaultDB:ConnStr");
            optionsBuilder.UseSqlServer(connection_str);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
