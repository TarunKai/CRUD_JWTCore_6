using FormulaOneCRUDAppCore6.Models;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneCRUDAppCore6.Data
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options)
        {
            
        }
        public DbSet<Team> Teams { get; set; }
    }
}
