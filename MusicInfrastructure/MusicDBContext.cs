using Microsoft.EntityFrameworkCore;
using MusicDomain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicInfrastructure
{
    public class MusicDBContext : DbContext
    {
        private readonly DbContextOptions<MusicDBContext> options;

        public DbSet<Music> Musics { get; private set; }//不要忘了写set，否则拿到的DbContext的Categories为null
        public DbSet<Album> Albums { get; private set; }
        public DbSet<Artist> Artists { get; private set; }
        public MusicDBContext(DbContextOptions<MusicDBContext> options) : base(options)
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
            modelBuilder.Entity<Music>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Album>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Artist>().HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
