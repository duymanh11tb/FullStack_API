using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QLDA_PCCV.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=DUYMANH;Database=QLDA_PCCV;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}
