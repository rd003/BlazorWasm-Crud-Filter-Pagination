using BlazorWasmCrud.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmCrud.Server.Data
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
                : base(options)
        {
        }

        public DbSet<Person> Person { get; set; }
    }
}
