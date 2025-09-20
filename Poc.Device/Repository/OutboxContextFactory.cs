using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Poc.Device.Repository;

public class OutboxContextFactory : IDesignTimeDbContextFactory<OutboxContext>
{
    public OutboxContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OutboxContext>();

        // SQLite connection string — change path as needed
         var connectionString = "Data Source=outbox.db";
      //  var connectionString =  "Server=localhost,1433;Database=outbox;User Id=sa;Password=Passw0rd!;TrustServerCertificate=True";

        optionsBuilder.UseSqlite(connectionString);

        return new OutboxContext(optionsBuilder.Options);
    }
}