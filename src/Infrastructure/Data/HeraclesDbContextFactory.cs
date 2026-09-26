using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Heracles.Infrastructure.Data
{
    public sealed class HeraclesDbContextFactory : IDesignTimeDbContextFactory<HeraclesDbContext>
    {
        public HeraclesDbContext CreateDbContext(string[] args) {

            var options = new DbContextOptionsBuilder<HeraclesDbContext>()
                .UseSqlite("Data Source=C:\\Projects\\Heracles\\.data\\heracles.db")
                .Options;

            return new HeraclesDbContext(options);
        }
    }
}