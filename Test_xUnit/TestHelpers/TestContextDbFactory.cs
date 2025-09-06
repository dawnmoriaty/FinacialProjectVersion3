using Microsoft.EntityFrameworkCore;

namespace Test_xUnit.TestHelper;
using FinacialProjectVersion3.Data;
public static  class TestContextDbFactory
{
    public static ApplicationDbContext CreateDbContext(string? databaseName = null)
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = $"TestDb_{Guid.NewGuid()}";
        }
            
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
                
        var context = new ApplicationDbContext(options);
            
        return context;
    }
}