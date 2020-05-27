using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MusicBot.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<SearchTrackMain> Musicinfo { get; set; }
        public ApplicationContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=Mydatabase; Trusted_Connection=true;") ;
        }
    }
}
